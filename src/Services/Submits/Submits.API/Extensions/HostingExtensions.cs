#region

using System.Reflection;
using HimuOJ.Common.WebHostDefaults.Extensions;
using HimuOJ.Services.Submits.API.Application.Queries;
using HimuOJ.Services.Submits.API.Hubs;
using HimuOJ.Services.Submits.API.Services;
using HimuOJ.Services.Submits.Infrastructure;
using HimuOJ.Services.Submits.Infrastructure.Repositories;
using Serilog;

#endregion

namespace HimuOJ.Services.Submits.API.Extensions;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.ConfigureDefaultPorts();

        Log.Information("Adding database connection for {type}", nameof(SubmitsDbContext));
        builder.Services.AddDatabaseConnection<SubmitsDbContext>(builder.Configuration);

        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        builder.Services.AddScoped<ISubmitsRepository, SubmitsRepository>();
        builder.Services.AddScoped<ISubmitsQuery, SubmitsQuery>();

        builder.Services.AddGrpc();
        builder.Services.AddControllers();

        builder.AddDefaultOpenApi();
        builder.AddDefaultAuthenticationPolicy();

        // Event Bus
        builder.AddEventBus<SubmitsDbContext>();
        builder.Services.AddScoped<IEventBusService, EventBusService>();

        builder.Services.AddSignalR();
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseDefaultOpenApi();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<SubmissionStatusHub>("/submitshub");
        return app;
    }
}