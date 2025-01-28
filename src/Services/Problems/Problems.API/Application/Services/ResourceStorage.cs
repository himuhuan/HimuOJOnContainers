using HimuOJ.Common.BucketStorage;


namespace HimuOJ.Services.Problems.API.Application.Services;

public class ResourceStorage : IResourceStorage
{
    private readonly IBucketStorage _storage;

    public ResourceStorage(IBucketStorage storage)
    {
        _storage = storage;
    }

    public async Task<string> UploadExpectedOutputFileAsync(int problemId, IFormFile expectedOutputFile)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string fileName = $"{timestamp}.ans";
        string path = Path.Combine("problems", problemId.ToString(), fileName);
        using var stream = expectedOutputFile.OpenReadStream();
        await _storage.UploadAsync(stream, path, expectedOutputFile.Length, "text/plain");
        return fileName;
    }

    public async Task<string> UploadInputFileAsync(int problemId, IFormFile inputFile)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string fileName = $"{timestamp}.in";
        string path = Path.Combine("problems", problemId.ToString(), fileName);
        using var stream = inputFile.OpenReadStream();
        await _storage.UploadAsync(stream, path, inputFile.Length, "text/plain");
        return fileName;
    }

    public async Task<Stream> DownloadResourceAsync(int problemId, string fileName)
    {
        string path = Path.Combine("problems", problemId.ToString(), fileName);
        if (!await _storage.IsFileExits(path))
        {
            throw new FileNotFoundException();
        }
        return await _storage.DownloadAsync(path);
    }
}
