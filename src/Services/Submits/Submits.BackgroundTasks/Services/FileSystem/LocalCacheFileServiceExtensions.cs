using Serilog;

namespace Submits.BackgroundTasks.Services;

public static class LocalCacheFileServiceExtensions
{
    public static IServiceCollection ConfigureLocalCacheFileService(
        this IServiceCollection services,
        IConfigurationSection section)
    {
        services.Configure<LocalCacheFileServiceOptions>(section);
        var workspaceDir = section.GetValue<string>("CacheDirectory") ?? "workspace";
        if (!Directory.Exists(workspaceDir))
        {
            Log.Warning("Cache directory {CacheDirectory} does not exist, creating it", workspaceDir);
            Directory.CreateDirectory(workspaceDir);
        }
        services.AddSingleton<ILocalCacheFileService, LocalCacheFileService>();
        return services;
    }
}