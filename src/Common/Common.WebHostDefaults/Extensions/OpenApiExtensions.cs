#region

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using HimuOJ.Common.WebHostDefaults.Infrastructure.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
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
        //   "XmlComments": "xxx.xml",
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
        var serviceUrl = openApiOptions["ServiceUrl"] ?? "http://localhost:5000";
        var xmlFile = openApiOptions["XmlComments"];
        var scopes = identityServerOptions.GetRequiredSection("Scopes")
            .GetChildren()
            .ToDictionary(x => x.Key, x => x.Value);

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(version, new OpenApiInfo { Title = name, Version = version });

            if (!identityServerOptions.Exists())
                return;

            string url = identityServerOptions.GetValue<string>("Url");

            c.AddServer(new OpenApiServer { Url = serviceUrl });
            
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{url}/connect/authorize"),
                        TokenUrl         = new Uri($"{url}/connect/token"),
                        Scopes           = scopes,
                    }
                }
            });

            c.OperationFilter<AuthenticationOperationFilter>([scopes.Keys.ToArray()]);

            // Set the comments path for the Swagger JSON and UI.
            if (xmlFile != null)
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath, true);
                }
            }
        });

        return builder;
    }

    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        var openApiSection = app.Configuration.GetSection("OpenApi");
        var identityServerSection = app.Configuration.GetSection("IdentityServer");
        
        app.UseSwagger();

        if (app.Environment.IsDevelopment() && openApiSection.Exists())
        {
            var scopes = identityServerSection.GetRequiredSection("Scopes")
                .GetChildren()
                .Select(x => x.Key)
                .ToArray();
         
            var clientId = openApiSection["Auth:ClientId"];

            app.UseSwagger(options => { options.RouteTemplate = "/swagger/v1/swagger.json"; });

            app.MapScalarApiReference(options =>
            {
                options.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecurityScheme = "oauth2",
                    OAuth2 = new OAuth2Options
                    {
                        ClientId = clientId,
                        Scopes   = scopes
                    }
                };
                options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
                options.WithEndpointPrefix("/api-reference/{documentName}");
            });

            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
        }

        return app;
    }
    
}