using HimuOJ.Common.BucketStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Common.BucketStorage.Minio;

public static class MinioBucketStorageExtensions
{
    public static IServiceCollection AddBucketStorage(this IServiceCollection services, IConfigurationSection section)
    {
        services.Configure<BucketStorageOptions>(section);
        var storage = section.Get<BucketStorageOptions>();
        services.AddMinio(client => {
            client.WithEndpoint(storage.Endpoint)
                .WithCredentials(storage.AccessKey, storage.SecretKey)
                .WithSSL(storage.UseSSL)
                .Build();
        });
        services.AddSingleton<IBucketStorage, MinioBucketStorage>();
        return services;
    }
}