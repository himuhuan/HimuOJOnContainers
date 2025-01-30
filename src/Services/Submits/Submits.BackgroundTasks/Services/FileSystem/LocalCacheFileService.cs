#region

using HimuOJ.Common.BucketStorage;
using Microsoft.Extensions.Options;

#endregion

namespace Submits.BackgroundTasks.Services;

public class LocalCacheFileService : ILocalCacheFileService
{
    private readonly LocalCacheFileServiceOptions _options;
    private readonly ILogger<LocalCacheFileService> _logger;
    private readonly IBucketStorage _storage;

    public LocalCacheFileService(IOptions<LocalCacheFileServiceOptions> options,
                                 ILogger<LocalCacheFileService> logger,
                                 IBucketStorage storage)
    {
        _options = options.Value;
        _logger = logger;
        _storage = storage;
    }

    public async Task<string> CreateOrGetAsync(
        string directory,
        string fileName,
        string content)
    {
        var filePath = Path.Combine(BasePath, directory);
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);
        filePath = Path.Combine(filePath, fileName);

        if (File.Exists(filePath))
        {
            _logger.LogInformation("{FileName} already exists, using cached file", fileName);
            return filePath;
        }
        _logger.LogInformation("{FileName} does not exist, creating new file", fileName);
        await File.WriteAllTextAsync(filePath, content);
        return filePath;
    }

    public string CombineAndMakeSureDirectoryExists(string path, string fileName)
    {
        var fullPath = Path.Combine(BasePath, path);
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);
        return Path.Combine(fullPath, fileName);
    }

    public string BasePath =>
        Path.Combine(Directory.GetCurrentDirectory(), _options.CacheDirectory);

    public async Task<string> CreateOrGetAsync(string fileName, string content)
    {
        var filePath = Path.Combine(BasePath, fileName);

        if (File.Exists(filePath))
        {
            _logger.LogInformation("{FileName} already exists, using cached file", fileName);
            return filePath;
        }

        await File.WriteAllTextAsync(filePath, content);
        return filePath;
    }

    public async Task<string> DownloadOrGetAsync(string directory, string fileName, string url)
    {
        var filePath = Path.Combine(BasePath, directory);
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);
        filePath = Path.Combine(filePath, fileName);

        if (File.Exists(filePath))
        {
            _logger.LogInformation("{FileName} already exists, using cached file", fileName);
            return filePath;
        }
        
        _logger.LogInformation("{FileName} does not exist, downloading from {Url}", fileName, url);
        var stream = await _storage.DownloadAsync(url);
        using var fileStream = File.Create(filePath);
        await stream.CopyToAsync(fileStream);
        return filePath;
    }
}