namespace Submits.BackgroundTasks.Services.Judge;

public sealed record CompilerConfiguration(
    string FullPath,
    TimeSpan Timeout,
    string Extension,
    string Template);

public class CompileServicesOptions
{
    public bool OutputCompilerMessageInLog { get; init; } = false;

    public required Dictionary<string, CompilerConfiguration> Compilers { get; init; }
}

public record CompilerResult(string Executable, int ExitCode, string Message);

public interface ICompileService
{
    CompilerResult? Compile(string compiler, string source, int submissionId);
}