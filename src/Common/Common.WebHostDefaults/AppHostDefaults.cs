#region

using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

#endregion

namespace HimuOJ.Common.WebHostDefaults;

public static class AppHostDefaults
{
    const string APP_LOGO =
        """
        [HimuOJ Starting]=============================================================================
                           .--
           +#.     -*-     +@@:                                .=*####=     +******+
          .@@      %@.  .::..       .  :.  .:.    .      .    +@#-..:+@#    .::::*@+
          +@-     :@*  ##**@*      +@=#*@*+#%@.  *@:    =@=  *@=      %@.        %@.
          %@#*****%@-      @@      #@%:.@@+ #@. .@%     %@. =@*       @@        -@*
         -@*:::::-@%      =@+     .@%  *@- .@*  =@=    =@*  #@:      +@+        #@:
         #@:     =@=      %@      =@-  @%  +@:  @@    =@@-  %@:     +@*        :@#
        :@#      @@.      @@--=+  @@  =@- .@%   %@=-=#*@@   -@@+--+%%=   *#=-=*@#.
        :=.     .=-       :=++=:  =:  -=  .=:   .=++=: --    .-+++=:     :=+++=.
        ============================================================[{ServiceName} Version {ApiVersion}]
        """;

    public static WebApplicationBuilder CreateBootstrapBuilder(
        string[] args,
        string serviceName,
        string apiVersion,
        bool showLogo = true)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .Console()
            .MinimumLevel
            .Debug()
            .CreateBootstrapLogger();

        Log.Information("Now starting HimuOJ {serviceName} {apiVersion}  ...", serviceName,
            apiVersion);
        if (showLogo)
        {
            string logo = APP_LOGO
                .Replace("{ApiVersion}", apiVersion)
                .Replace("{ServiceName}", serviceName)
                .Replace('\r', ' '); // better display for some terminals...
            Console.WriteLine(logo);
        }

        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, services, config) =>
        {
            config.ReadFrom
                .Configuration(context.Configuration)
                .ReadFrom
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
        return builder;
    }
}