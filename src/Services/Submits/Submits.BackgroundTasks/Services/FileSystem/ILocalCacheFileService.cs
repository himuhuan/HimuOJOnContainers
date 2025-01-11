namespace Submits.BackgroundTasks.Services;

/// <summary>
///     Cache file service for local file system.
/// </summary>
/// <remarks>
///     <para>
///         The cache file service is used to store some files can be reused for the judge task.
///         For example, the input file of the test point.
///     </para>
///     <para>
///         Use <see cref="File.Delete" /> to delete the file when it is no longer needed.
///     </para>
/// </remarks>
public interface ILocalCacheFileService
{
    public string BasePath { get; }
    public Task<string> CreateOrGetTextFileAsync(string fileName, string content);

    public Task<string> CreateOrGetTextFileAsync(string directory, string fileName, string content);

    public string CombineAndMakeSureDirectoryExists(string path, string fileName);
}