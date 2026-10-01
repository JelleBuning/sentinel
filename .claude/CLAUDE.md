# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Mycelium is a Remote Monitoring and Management (RMM) platform (.NET 10 / C# 13) for MSPs/IT pros
to monitor, manage, and secure client devices. It's a back-end-only solution: a central ASP.NET
Core API plus a cross-platform worker agent that runs on managed endpoints and talks to the API
in real time over SignalR.

## Workflow

Use /implement-issue <number> to pick up a GitHub issue: plan → wait for approval → implement + tests → report done. Opening a PR is a separate, explicit step the user decides on — never done automatically.

### Automated dev-cycle

`/dev-cycle` picks the oldest `refined`-labeled issue and drives it through plan → implement → test → review via the subagents in `.claude/agents/`, stopping for approval before code and before any PR; it never merges. Run it on a loop: `/loop 5m /dev-cycle`.

`refined` is applied **by hand**; /dev-cycle moves it to `in-progress` → `in-review`. An aborted run leaves it on `in-progress` so the loop won't re-pick it.

This repo's actual labels are: `bug`, `documentation`, `enhancement`, `duplicate`, `good first issue`, `help wanted`, `invalid`, `question`, `wontfix`, `dependencies`, `github_actions`, plus bare `refined`/`in-progress`/`in-review` (no `status:`/`type:`/`area:`/`size:` prefixes). Don't invent a label scheme that doesn't exist here — check `gh label list` before applying labels to an issue.

### Branch naming

Branches follow `(main|(features|bugs|hotfix)\/[0-9]+-.+)` when tied to an issue:
- `main` — the default branch.
- `features/<issue-number>-<slug>` — new functionality.
- `bugs/<issue-number>-<slug>` — fixing incorrect/unwanted existing behavior.
- `hotfix/<issue-number>-<slug>` — urgent fixes.

Some existing branches predate this convention (e.g. `feature/89-rebrand-sentinel-to-mycelium` uses singular `feature/`) — that's historical, not something to imitate for new work tied to an issue.

The `EnterWorktree` tool always prefixes branch names with `worktree-`, which breaks this convention. After creating a worktree, immediately rename the branch with `git branch -m <features|bugs|hotfix>/<issue-number>-<slug>` before making any commits.

## Commands

```bash
# Build the whole solution
dotnet build Mycelium.slnx

# Run the API
dotnet run --project src/Mycelium.Api

# Run the worker agent
dotnet run --project src/Mycelium.WorkerService

# Run the tests
dotnet test tests/Mycelium.Api.Integration.Tests

# Run a single test
dotnet test --filter "FullyQualifiedName~SomeFixture.SomeMethod"
```

Tests use **NUnit** (`NUnit3TestAdapter`) with `Microsoft.AspNetCore.Mvc.Testing` (WebApplicationFactory-style integration tests against `Mycelium.Api`) and `Microsoft.EntityFrameworkCore.InMemory`. There is currently a single test project, `tests/Mycelium.Api.Integration.Tests` — no separate unit test project exists yet.

Package versions are centrally managed in `Directory.Packages.props` (`ManagePackageVersionsCentrally=true`) — never add a `Version` attribute to a `PackageReference` in a `.csproj`; add/bump the version in `Directory.Packages.props` instead.

CI (`.github/workflows/dotnet.yml`) builds and tests on a ubuntu/windows/macos matrix on every push. Tags matching `v*.*.*[-beta[.N]]` trigger `.github/workflows/dotnet_release.yml`, which publishes self-contained `Mycelium.Api` builds and a Docker image to `ghcr.io/<repo>-api`.

## Architecture

**Vertical-slice API + separate worker agent, no shared solution-wide layering.**

```
src/
  Mycelium.Api                       ← ASP.NET Core host (entry point, Program.cs)
  Mycelium.Api.Core                  ← shared endpoint/Result infra, API versioning, OpenApi/Scalar
  Mycelium.Api.Auth                  ← auth feature slice (JWT issuing, 2FA/OTP, etc.)
  Mycelium.Api.Devices                ← device management + SignalR device hub feature slice
  Mycelium.Api.Organisations          ← organisation management feature slice
  Mycelium.Api.Users                  ← user management feature slice
  Mycelium.Api.EntityFramework        ← EF Core + SQL Server persistence
  Mycelium.Common                     ← shared SignalR message contracts + auth abstractions, referenced by both API and worker
  Mycelium.WorkerService              ← cross-platform worker agent host (runs on managed endpoints)
  Mycelium.WorkerService.Common
  Mycelium.WorkerService.Core
  Mycelium.WorkerService.Core.Linux   ← Linux-specific worker implementation
  Mycelium.WorkerService.Core.Windows ← Windows-specific worker implementation
  Mycelium.WorkerService.RemoteAccess ← remote-access feature (PowerShell, service control) for the worker
tests/
  Mycelium.Api.Integration.Tests      ← the only test project
```

Each `Mycelium.Api.*` feature project is organised as **vertical slices**: one folder per use case,
versioned (`v1`, `v2`, …), each containing a `Command`/`Query`, an `Endpoint`, and a `Handler` —
e.g. `Mycelium.Api.Devices/ExecuteSecurityScan/v1/{Command,Endpoint,Handler}.cs`. There is no
Domain/Application/Infrastructure layering; cross-cutting endpoint/Result plumbing lives in
`Mycelium.Api.Core` instead.

The API and the worker agent talk over **SignalR**: message contracts live in
`Mycelium.Common/SignalR/*` (e.g. `RemoteAccessMessage`, `RestartDeviceMessage`,
`SecurityScanMessage`), the API side is `Mycelium.Api.Devices/SignalR/DeviceMessageHub.cs` +
`IDeviceMessenger`/`SignalRDeviceMessenger`, and the worker side is `HubManager.cs` in
`Mycelium.WorkerService`.

## Key conventions

- `sealed` on every class not designed for inheritance; `record` for commands, queries, and DTOs.
- **Mediator** (source-generated, package `Mediator.Abstractions`/`Mediator.SourceGenerator`) for CQRS — **not MediatR**.
- Endpoints implement `IEndpoint` (`MapEndpoint(IEndpointRouteBuilder)`); they're auto-registered via `app.MapEndpoints()` in `Mycelium.Api.Core` — never wire a route manually in `Program.cs`.
- **FluentValidation** for request validation.
- Handlers return `Result<T>` (`Mycelium.Api.Core/Results/Result.cs`), mapped to HTTP via `result.ToHttpResult()` — don't throw for expected failure cases.
- Handlers are `ValueTask<T>`, not `Task<T>`.
- Async methods take a `CancellationToken` where cancellation is meaningful (network/IO/DB calls).
- No comments in code at all.
- Prefer `var` by default; no suppressing nullable warnings.
- `ImplicitUsings` disabled — write explicit `using` directives.

## Local secrets / hooks

A `PreToolUse` hook (`.claude/hooks/block-secrets.py`) and a `Stop` hook
(`.claude/hooks/verify-no-secrets.sh`) already guard against committing secrets in this repo —
don't bypass or disable them. `git commit` and `git push` are permission-gated (`ask`) in
`.claude/settings.json`, so they'll always prompt rather than run silently. Never touch
`appsettings.Development.json` or other local-secret files.
