#region

using HimuOJ.Common.WebHostDefaults.Extensions;
using HimuOJ.Services.Problems.API.Application.Queries;
using HimuOJ.Services.Problems.API.GrpcServices;
using HimuOJ.Services.Problems.API.Infrastructure;
using HimuOJ.Services.Problems.Infrastructure;
using HimuOJ.Services.Problems.Infrastructure.Repositories;
using Serilog;

#endregion

namespace HimuOJ.Services.Problems.API;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.ConfigureDefaultPorts();

        Log.Information("Adding database connection for {type}", nameof(ProblemsDbContext));
        builder.Services.AddDatabaseConnection<ProblemsDbContext>(builder.Configuration);

        Log.Information("Applying seeder migration for {type}", nameof(ProblemsDbContext));
        builder.Services.AddDbContextMigration<ProblemsDbContext, ProblemsDbContextSeeder>();

        builder.Services.AddScoped<IProblemsRepository, ProblemsRepository>();
        builder.Services.AddScoped<IProblemsQuery, ProblemsQuery>();

        builder.Services.AddControllers();
        builder.Services.AddGrpc();

        builder.AddDefaultOpenApi();
        builder.AddDefaultAuthenticationPolicy();

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
        app.MapGrpcService<ProblemsGrpcServices>();

        return app;
    }
}