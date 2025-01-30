
namespace HimuOJ.Services.Problems.API.Application.Services;

public interface IResourceStorage
{
    public Task<string> UploadInputFileAsync(int problemId, IFormFile inputFile);

    public Task<string> UploadExpectedOutputFileAsync(int problemId, IFormFile expectedOutputFile);
    Task<Stream> DownloadResourceAsync(int problemId, string fileName);
    string GetResourceUrl(int problemId, string fileName);
}
