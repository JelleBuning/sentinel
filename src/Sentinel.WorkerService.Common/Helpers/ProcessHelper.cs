using System.Diagnostics;

namespace Sentinel.WorkerService.Common.Helpers;

public static class ProcessHelper
{
    public static Process Start(string fileName, string arguments)
    {
        var process = new Process();
        var startInfo = new ProcessStartInfo(fileName)
        {
            WorkingDirectory = Path.GetDirectoryName(fileName),
            FileName = Path.GetFileName(fileName),
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            Arguments = arguments
        };
        process.StartInfo = startInfo;
        process.Start();
        return process;
    }
}