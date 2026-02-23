# Copilot Instructions (C#/.NET) — SOLID & Design Patterns First

## Mission
Produce maintainable, testable, evolvable C# code. **Top priority is SOLID** and applying **appropriate design patterns** over “quick fixes”.

## Highest Priority Rules (Always)
1. **Correctness > Maintainability > Performance** (unless explicitly told otherwise).
2. **SOLID first**:
   - **S**RP: one reason to change per type/module.
   - **O**CP: extend with new behavior via abstractions, not by editing many call sites.
   - **L**SP: derived types must not break expectations of base types/interfaces.
   - **I**SP: small, role-focused interfaces; avoid “god” interfaces.
   - **D**IP: depend on abstractions; inject dependencies; avoid static/global state.
3. Prefer **composition over inheritance**.
4. Keep dependencies pointing inward (domain/core should not depend on infrastructure).
5. Make changes **small, reversible, and well-tested**.

## Core Design Principles
- **KISS** (Keep It Simple, Stupid): Prefer straightforward solutions. Avoid unnecessary complexity, over-engineering, and "clever" code. Clarity and maintainability trump cleverness.
- **DRY** (Don't Repeat Yourself): Eliminate code duplication through abstractions, helper methods, and reusable components. Duplicated logic is a maintenance risk and divergence hazard.
- **YAGNI** (You Aren't Gonna Need It): Don't add features, parameters, or abstractions you don't need today. Speculative generalization introduces complexity; add them when you actually need them.

## Domain-Driven Design (DDD)
Structure the domain model to reflect business reality:
- **Ubiquitous Language**: Use domain terms consistently across code, conversations, and documentation. Align naming with how domain experts speak.
- **Entities**: Objects with identity and lifecycle; contain business logic tied to their identity.
- **Value Objects**: Immutable, identity-less objects representing domain concepts (e.g., `Money`, `PhoneNumber`). Prefer these when identity doesn't matter.
- **Aggregates**: Cohesive clusters of entities/value objects with a root entity (`AggregateRoot`) that maintains consistency boundaries.
  - Model only the aggregates and relationships the business needs; avoid god aggregates.
  - Each aggregate should be independently testable.
- **Domain Services**: Stateless operations that don't naturally belong to an entity or value object; they orchestrate domain logic across aggregates.
- **Domain Events**: Capture significant business occurrences (e.g., `UserRegistered`, `OrderPlaced`). Publish them to trigger side effects or integration points.
- **Repositories**: Abstractions that represent collections of aggregates; hide persistence details. Retrieve/persist only aggregate roots.
- **Bounded Contexts**: Define clear boundaries where specific ubiquitous language applies. Different contexts may model the same concept differently; use Anti-Corruption Layers or Context Mappers at boundaries.

## Architectural Defaults (Unless the repo says otherwise)
- Prefer **Clean Architecture / Onion** style aligned with **Domain-Driven Design**:
  - **Domain/Core**: entities, value objects, domain services, domain events, aggregates, and business rules (no infrastructure dependencies).
  - **Application**: use-cases, orchestration, application services, ports (interfaces), DTOs, and command/query handlers.
  - **Infrastructure**: EF Core, HTTP clients, file system, external services, and repository implementations.
  - **Presentation**: Web API / UI / Controllers.
- Use **dependency injection** (Microsoft.Extensions.DependencyInjection).
- Avoid leaking infrastructure types (e.g., EF `DbContext`, `IQueryable`) out of infrastructure.

## Design Patterns: When to Use What
Apply patterns intentionally—don’t “pattern-fest”.

- **Strategy**: interchangeable algorithms; choose at runtime (e.g., pricing rules).
- **Factory / Abstract Factory**: complex creation, invariants, environment-specific implementations.
- **Decorator**: cross-cutting behavior (caching, retries) without modifying core services.
- **Adapter**: wrap external SDKs to stable internal interfaces.
- **Facade**: simplify interactions with a complex subsystem.
- **Command**: represent actions/use-cases; supports logging, retries, queues.
- **Mediator (MediatR)**: for request/response or notifications in app layer (only if already used).
- **Repository**: only if it adds value over EF Core usage and boundaries are clear.
- **Unit of Work**: usually EF Core already is; avoid double-abstraction.
- **Specification**: reusable query predicates/validation rules.
- **Observer**: domain events, integration events, notifications.
- **State**: complex state transitions.

## C#/.NET Coding Standards
- Use modern C# features appropriately: `record` for immutable DTOs/value objects, `init`, pattern matching.
- Prefer immutability by default.
- Prefer `IReadOnlyList<T>` / `IReadOnlyCollection<T>` for outward-facing collections.
- Avoid `async void` (except event handlers). Prefer `CancellationToken` on async APIs.
- **Never use `.Result` or `.Wait()` on tasks** — this causes deadlocks and defeats async/await benefits. Always `await` instead.
- Use expression-bodied members for simple getters/methods.
- Use `using` declarations for disposables when possible.
- Use guard clauses; validate public method arguments.
- Keep public APIs small; internal helpers private/internal.

### Naming
- Types/Methods: PascalCase; locals: camelCase.
- Interfaces: `IThing`.
- Async methods: `ThingAsync`.
- Tests: `Method_Scenario_ExpectedResult` or `Given_When_Then`.

## Error Handling & Results
- For application/service boundaries, prefer explicit result types:
  - `Result<T>` / `OneOf` / `ErrorOr<T>` (use what the repo already uses).
- Use exceptions for truly exceptional conditions; include context.

## Testing Expectations
- Prefer **unit tests** for business logic; integration tests for persistence/HTTP.
- Arrange-Act-Assert (AAA).
- Test behavior, not implementation details.
- For time/randomness/IO, inject abstractions (e.g., `ISystemClock`, `IRandom`, file interfaces).
- When changing behavior, **add/adjust tests first** where feasible.

## Performance Guidance
- Don’t micro-optimize.
- Avoid unnecessary allocations in hot paths; use `Span<T>` only when justified and readable.
- Use streaming for large payloads; avoid loading whole files into memory.
- Prefer `IAsyncEnumerable<T>` for large sequences when appropriate.

## Security & Reliability
- Treat external input as untrusted; validate and sanitize.
- Avoid string concatenation for SQL; use parameterized queries/EF.
- Use `HttpClientFactory`; set timeouts; handle transient failures (policies if used).
- Don’t log secrets/PII; prefer structured logging.

## Git & Change Discipline
When asked to implement something:
1. **Clarify requirements** (inputs/outputs, edge cases, constraints) if ambiguous.
2. **Propose a minimal design**:
   - responsibilities
   - key abstractions/interfaces
   - chosen pattern (if any) and why
3. **Implement in small commits** (or small logical steps) if working interactively.
4. **Add/adjust tests**.
5. **Explain tradeoffs** briefly (why this pattern, why these boundaries).

## “Stop and Ask” Triggers
Ask before proceeding if:
- A choice changes public API shape, persistence schema, or cross-service contracts.
- A new dependency/package is needed.
- There are multiple plausible patterns/architectures and the repo conventions aren’t clear.
- Behavior impacts security, authz/authn, or financial logic.

## Output Format Expectations
- Prefer producing complete compilable code with necessary `using`s and namespaces.
- Keep diffs focused; don’t reformat unrelated code.
- If unsure of existing conventions, follow EditorConfig / analyzers in the repo.

## Quick SOLID Checklist (Self-Review)
- Does each class have a single responsibility?
- Are new behaviors added by extension rather than modifying many existing files?
- Can substitutes be used without surprises?
- Are interfaces minimal and role-based?
- Do higher-level modules depend on abstractions rather than concretions?