#region

using HimuOJ.Common.WebHostDefaults.Infrastructure.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

#endregion

namespace HimuOJ.Common.WebHostDefaults.Extensions;

public static class OpenApiExtensions
{
    // TODO: add Api Versioning support
    public static IHostApplicationBuilder AddDefaultOpenApi(this IHostApplicationBuilder builder)
    {
        //
        // "OpenApi": {
        //   "Name": "HimuOJ.xxx.API",
        //   "Version": "v1",
        //   "Auth": {
        //       "ClientId": "xxx",
        //       "ClientName": "xxx",
        //       "ClientSecret": "xxx"
        //   }
        // },
        // "IdentityServer": {
        //   "Url": "https://localhost:5001",
        //   "Scopes": {
        //      "xxx": "xxx",
        //      "xxx": "xxx"
        //   }
        // }
        //

        // ArgumentNullException.ThrowIfNull(apiVersioning);

        var openApiOptions        = builder.Configuration.GetSection("OpenApi");
        var identityServerOptions = builder.Configuration.GetSection("IdentityServer");
        if (!openApiOptions.Exists())
        {
            Log.Warning("OpenApi or IdentityServer section not found in configuration, skipped");
            return builder;
        }

        builder.Services.AddEndpointsApiExplorer();

        var name    = openApiOptions["Name"];
        var version = openApiOptions["Version"];
        var scopes = identityServerOptions.GetRequiredSection("Scopes")
            .GetChildren()
            .ToDictionary(x => x.Key, x => x.Value);

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(version, new OpenApiInfo { Title = name, Version = version });

            if (!identityServerOptions.Exists())
                return;

            string url = identityServerOptions.GetValue<string>("Url");

            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{url}/connect/authorize"),
                        TokenUrl         = new Uri($"{url}/connect/token"),
                        Scopes           = scopes
                    }
                }
            });

            c.OperationFilter<AuthenticationOperationFilter>([scopes.Keys.ToArray()]);
        });

        return builder;
    }

    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        var openApiSection = app.Configuration.GetSection("OpenApi");

        app.UseSwagger();

        if (app.Environment.IsDevelopment() && openApiSection.Exists())
        {
            app.UseSwaggerUI(setup =>
            {
                var authOptions = openApiSection.GetSection("Auth");
                if (authOptions.Exists())
                {
                    setup.OAuthClientId(authOptions["ClientId"]);
                    setup.OAuthAppName(authOptions["ClientName"]);
                }
            });

            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
        }

        return app;
    }
}