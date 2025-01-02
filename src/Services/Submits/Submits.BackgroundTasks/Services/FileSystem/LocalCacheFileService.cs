using Microsoft.Extensions.Options;

namespace Submits.BackgroundTasks.Services;

public class LocalCacheFileService : ILocalCacheFileService
{
    private LocalCacheFileServiceOptions _options;

    public LocalCacheFileService(IOptions<LocalCacheFileServiceOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> CreateOrGetTextFileAsync(string directory, string fileName, string content)
    {
        var filePath = Path.Combine(BasePath, directory);
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);
        
        filePath = Path.Combine(filePath, fileName);
        if (File.Exists(filePath))
        {
            return filePath;
        }
        
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

    public string BasePath => Path.Combine(Directory.GetCurrentDirectory(), _options.CacheDirectory);

    public async Task<string> CreateOrGetTextFileAsync(string fileName, string content)
    {
        var filePath = Path.Combine(BasePath, fileName);
        
        if (File.Exists(filePath))
        {
            return filePath;
        }
        
        await File.WriteAllTextAsync(filePath, content);
        return filePath;
    }
}