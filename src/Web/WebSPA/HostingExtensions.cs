#region

using Duende.Bff.Yarp;
using HimuOJ.Web.WebSPA.Filters;
using HimuOJ.Web.WebSPA.Services;
using Microsoft.IdentityModel.Logging;
using Refit;
using Serilog;

#endregion

namespace HimuOJ.Web.WebSPA;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddBff().AddRemoteApis();

        builder.Services.AddReverseProxy()
            .AddBffExtensions()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        var identityServer = builder.Configuration.GetRequiredSection("IdentityServer");

        string identityServerUrl = identityServer.GetValue<string>("Url")
                                   ?? throw new ArgumentException(
                                       "IdentityServer:Url is not configured");

        var scopes = identityServer.GetRequiredSection("Scopes")
            .GetChildren()
            .ToDictionary(x => x.Key, x => x.Value);

        Log.Information("Using Identity Server: {ServerUrl}", identityServerUrl);
        foreach (var scope in scopes)
        {
            Log.Information("Adding scope: {ScopeKey}: {ScopeValue}", scope.Key, scope.Value);
        }

        if (builder.Environment.IsDevelopment())
        {
            IdentityModelEventSource.ShowPII = true;
        }

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme          = "Cookies";
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignOutScheme   = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority    = identityServerUrl;
                options.ClientId     = "webspa";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                options.ResponseMode = "query";


                // Required for docker compose
#if DEBUG
                options.MetadataAddress = $"{identityServerUrl}/.well-known/openid-configuration";
                options.RequireHttpsMetadata = false;
#endif

                options.Scope.Clear();

                foreach (var scope in scopes)
                {
                    options.Scope.Add(scope.Key);
                }

                options.GetClaimsFromUserInfoEndpoint = true;
                options.MapInboundClaims              = false;
                options.SaveTokens                    = true;
                options.DisableTelemetry              = true;

                options.TokenValidationParameters = new()
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<BffGatewayRefitExceptionFilter>();
        });

        builder.Services.AddRemoteApis<IProblemsApi>();
        builder.Services.AddRemoteApis<ISubmitsApi>();
        builder.Services.AddRemoteApis<IUsersApi>();

        builder.Services.AddAuthorization();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        var configuration = app.Configuration;

        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseRouting();
        app.UseAuthentication();

        app.UseBff();

        app.UseAuthorization();

        app.MapReverseProxy(proxyPipeline => { proxyPipeline.UseAntiforgeryCheck(); });

        app.MapBffManagementEndpoints();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        return app;
    }

    private static IServiceCollection AddRemoteApis<TApiInterface>(this IServiceCollection services)
        where TApiInterface : class
    {
        services.AddRefitClient<TApiInterface>(new RefitSettings
            {
                // restful api supported only
                CollectionFormat = CollectionFormat.Multi
            })
            .ConfigureHttpClient(c =>
            {
                // redirect to gateway itself
                c.BaseAddress = new Uri("http://webspa-bff");
                c.DefaultRequestHeaders.Add("X-CSRF", "1");
            }); // TODO: retry policy
        return services;
    }
}