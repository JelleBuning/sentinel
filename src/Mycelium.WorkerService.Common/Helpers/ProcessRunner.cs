using System.Diagnostics;

namespace Mycelium.WorkerService.Common.Helpers;

public sealed class ProcessRunner : IProcessRunner
{
    public IProcessHandle Start(string fileName, string arguments)
    {
        return new ProcessHandle(ProcessHelper.Start(fileName, arguments));
    }
}

internal sealed class ProcessHandle(Process process) : IProcessHandle
{
    public int ExitCode => process.ExitCode;

    public string? ReadLine() => process.StandardOutput.ReadLine();

    public string ReadToEnd() => process.StandardOutput.ReadToEnd();

    public void WaitForExit() => process.WaitForExit();

    public Task WaitForExitAsync(CancellationToken cancellationToken = default) => process.WaitForExitAsync(cancellationToken);

    public void Dispose() => process.Dispose();
}
