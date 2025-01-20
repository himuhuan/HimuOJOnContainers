namespace HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;

/// <summary>
///     Compilers supported by the current service.
/// </summary>
/// <remarks>
///     The supported compilers are defined in the configuration file.
///     The <see cref="Submission.CompilerName" /> property is used to identify the compiler,
///     and the compiler name must be one of the supported compilers.
///     If the compiler name is not in the supported compilers
///     the submission will be rejected with code <c> NOT_SUPPORTED</c>
/// </remarks>
public class SupportedCompiler
{
    public string Name { get; init; }

    public string FullPath { get; init; }

    /// <summary>
    ///     The service use this template to generate the command line to compile the source code.
    /// </summary>
    /// <remarks>
    ///     {SourceFile} will be replaced by the full path of the source code file.
    /// </remarks>
    public string CommandLineTemplate { get; init; }

    /// <summary>
    ///     Timeout for the compiler. Default is 5 seconds.
    /// </summary>
    public TimeSpan Timeout { get; init; }
}