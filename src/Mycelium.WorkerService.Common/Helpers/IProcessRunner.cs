namespace Mycelium.WorkerService.Common.Helpers;

public interface IProcessRunner
{
    IProcessHandle Start(string fileName, string arguments);
}

public interface IProcessHandle : IDisposable
{
    int ExitCode { get; }
    string? ReadLine();
    string ReadToEnd();
    void WaitForExit();
    Task WaitForExitAsync(CancellationToken cancellationToken = default);
}
