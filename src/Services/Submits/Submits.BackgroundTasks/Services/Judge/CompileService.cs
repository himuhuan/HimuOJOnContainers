using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace Submits.BackgroundTasks.Services.Judge;

public class CompileService : ICompileService
{
    private readonly CompileServicesOptions _options;
    private readonly ILogger<CompileService> _logger;
    private readonly ILocalCacheFileService _files;

    public CompileService(
        IOptionsMonitor<CompileServicesOptions> options,
        ILogger<CompileService> logger,
        ILocalCacheFileService files)
    {
        _logger  = logger;
        _options = options.CurrentValue;
        _files   = files;
    }

    public CompilerResult? Compile(string compiler, string source, int submissionId)
    {
        string taskName = $"submission-{submissionId}";

        if (!_options.Compilers.TryGetValue(compiler, out var compilerOptions))
        {
            _logger.LogError("--- {taskId}: Compiler {Compiler} not found", taskName, compiler);
            return null;
        }

        string tempSourcePath =
            _files.CombineAndMakeSureDirectoryExists("sources", taskName + compilerOptions.Extension);
        string tempExecutablePath = _files.CombineAndMakeSureDirectoryExists("executables", taskName);
        string commandLine = compilerOptions.Template
                                            .Replace("{source}", tempSourcePath)
                                            .Replace("{output}", tempExecutablePath);
#if DEBUG
        if (File.Exists(tempSourcePath))
        {
            _logger.LogInformation("DEBUG MODE: using existed source file");
        }
        else
        {
            File.WriteAllText(tempSourcePath, source);
        }
#else
        File.WriteAllText(tempSourcePath, source);
#endif

        _logger.LogInformation("--- {TaskId}: Compiling source code with {Compiler} {Args}...",
            taskName, compiler, commandLine);

        Process process = new();
        process.StartInfo.FileName              = compilerOptions.FullPath;
        process.StartInfo.Arguments             = commandLine;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow        = true;
        process.Start();

        string compilerMessage = process.StandardError.ReadToEnd();
        if (!process.WaitForExit(compilerOptions.Timeout))
        {
            _logger.LogError("--- {TaskId}: Compilation timeout", taskName);
            return null;
        }

        _logger.LogInformation("--- {TaskId}: Compilation finished with exit code {ExitCode}", taskName,
            process.ExitCode);

#if !DEBUG
        File.Delete(tempSourcePath);
        File.Delete(tempExecutablePath);
#endif

        return new CompilerResult(tempExecutablePath, process.ExitCode, compilerMessage);
    }
}