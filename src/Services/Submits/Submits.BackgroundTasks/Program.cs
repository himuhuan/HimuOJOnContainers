using Serilog;
using Submits.BackgroundTasks.Extensions;

Log.Logger = new LoggerConfiguration()
             .WriteTo
             .Console()
             .MinimumLevel
             .Debug()
             .CreateBootstrapLogger();

Log.Information("Starting Submits Background Tasks Service...");

var builder = Host.CreateApplicationBuilder(args);

builder.ConfigureServices()
       .ConfigurePipeline()
       .Run();