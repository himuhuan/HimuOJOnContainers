using HimuOJ.Common.BucketStorage;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Common.BucketStorage.Minio;

public class MinioBucketStorage : IBucketStorage
{
    private readonly BucketStorageOptions _options;
    private readonly IMinioClient _client;

    public MinioBucketStorage(IOptions<BucketStorageOptions> options, IMinioClient client)
    {
        _options = options.Value;
        _client = client;
    }

    private async Task EnsureBucketExistsAsync()
    {
        var beArgs = new BucketExistsArgs()
                .WithBucket(_options.BucketName);
        bool found = await _client.BucketExistsAsync(beArgs).ConfigureAwait(false);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs()
                .WithBucket(_options.BucketName);
            await _client.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        }
    }

    private async Task<bool> IsBucketExistsAsync()
    {
        var args = new BucketExistsArgs().WithBucket(_options.BucketName);
        return await _client.BucketExistsAsync(args).ConfigureAwait(false);
    }

    public async Task<bool> DeleteAsync(string fileName)
    {
        if (!await IsBucketExistsAsync())
            return false;

        await _client.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(fileName));
        return true;
    }

    public async Task<Stream> DownloadAsync(string fileName)
    {
        MemoryStream ms = new();
        await _client.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(fileName)
            .WithCallbackStream(stream => { stream.CopyTo(ms); }));
        ms.Position = 0;
        return ms;
    }

    public async Task<bool> UploadAsync(Stream stream, string fileName, long size, string contentType)
    {
        await EnsureBucketExistsAsync();
        var putObjectArgs = new PutObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(size)
                .WithContentType(contentType);
        await _client.PutObjectAsync(putObjectArgs);
        return true;
    }

    public async Task<bool> IsFileExits(string fileName)
    {
        if (!await IsBucketExistsAsync())
            return false;
        StatObjectArgs statObjectArgs = new StatObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(fileName);
        try
        {
            await _client.StatObjectAsync(statObjectArgs);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}