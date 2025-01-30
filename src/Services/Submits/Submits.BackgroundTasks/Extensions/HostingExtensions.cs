#region

using System.Reflection;
using Common.BucketStorage.Minio;
using Grpc.Net.Client;
using GrpcProblems;
using HimuOJ.Common.WebHostDefaults.Extensions;
using HimuOJ.Services.Submits.Infrastructure;
using HimuOJ.Services.Submits.Infrastructure.Repositories;
using Serilog;
using Serilog.Events;
using Submits.BackgroundTasks.Services;
using Submits.BackgroundTasks.Services.Judge;
using Submits.BackgroundTasks.Services.Remote;
using Submits.BackgroundTasks.Services.Sandbox;
using Submits.BackgroundTasks.Services.Subscribers;

#endregion

namespace Submits.BackgroundTasks.Extensions;

public static class HostingExtensions
{
    public static IHost ConfigureServices(this HostApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, config) =>
        {
            config.ReadFrom
                .Services(services)
                .MinimumLevel
                .Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel
                .Override("Microsoft.EntityFrameworkCore", LogEventLevel.Verbose)
                .Enrich
                .FromLogContext()
                .WriteTo
                .Console();
        });

        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        builder.Services.AddDatabaseConnection<SubmitsDbContext>(builder.Configuration);
        builder.Services.AddScoped<ISubmitsRepository, SubmitsRepository>();

        builder.AddEventBus(builder.Configuration.GetConnectionString(nameof(SubmitsDbContext)));
        builder.Services.AddTransient<ISubmitsSubscriberService, SubmitsSubscriberService>();

        builder.ConfigureGrpcService<ProblemsService.ProblemsServiceClient>("ProblemsGrpcUrl")
            .AddSingleton<ProblemsServices>();

        builder.Services.ConfigureLocalCacheFileService(
            builder.Configuration.GetRequiredSection("LocalCacheFile"));

        // builder.Services.Configure<SandboxServicesOptions>(builder.Configuration.GetSection("SandboxServices"));
        builder.Services.AddSingleton<ISandboxService, SandboxService>();

        builder.Services.Configure<CompileServicesOptions>(
            builder.Configuration.GetRequiredSection("CompileServices"));
        builder.Services.AddSingleton<ICompileService, CompileService>();

        builder.Services.AddScoped<IJudgeService, JudgeService>();

        var storageOptions = builder.Configuration.GetSection("Storage");
        builder.Services.AddBucketStorage(storageOptions);

        return builder.Build();
    }

    public static IHost ConfigurePipeline(this IHost app)
    {
        /*var judgeService = app.Services.GetRequiredService<IJudgeService>();
        judgeService.AddJudgeTask(201);*/
        return app;
    }

    private static async Task<bool> WaitForGrpcServiceReady(string address, TimeSpan timeout)
    {
        using var channel = GrpcChannel.ForAddress(address);
        Log.Information("Waiting for gRPC service {Url} to be ready...", address);
        var deadlineTask = Task.Delay(timeout);
        var client = new ProblemsService.ProblemsServiceClient(channel);
        while (true)
        {
            try
            {
                var response = await client.CheckHealthAsync(new CheckHealthRequest());
                if (response.Status != 0)
                {
                    Log.Fatal("gRPC service {Url} reports unexpected code: {Code}", address,
                        response.Status);
                    return false;
                }

                Log.Information("gRPC service {Url} ready! Starting hosting services...", address);
                return true;
            }
            catch
            {
                if (await Task.WhenAny(deadlineTask, Task.Delay(1000)) == deadlineTask)
                {
                    return false;
                }
            }
        }
    }

    private static IServiceCollection ConfigureGrpcService<TClient>(
        this IHostApplicationBuilder builder,
        string configKey) where TClient : class
    {
        var grpcAddress = builder.Configuration.GetValue<string>(configKey)
                          ?? throw new InvalidOperationException($"{configKey} is not configured");

        Log.Information("Using GRPC service {ApiName} on {Url}", typeof(TClient).Name, grpcAddress);
        builder.Services.AddGrpcClient<TClient>(o => { o.Address = new Uri(grpcAddress); });

        var waitingForService = WaitForGrpcServiceReady(grpcAddress, TimeSpan.FromSeconds(30))
            .GetAwaiter()
            .GetResult();

        if (!waitingForService)
        {
            Log.Fatal("gRPC service {Url} is not ready", grpcAddress);
            throw new InvalidOperationException("gRPC service is not ready");
        }

        return builder.Services;
    }
}