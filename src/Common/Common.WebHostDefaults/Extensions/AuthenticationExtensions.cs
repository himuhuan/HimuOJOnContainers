using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics.Contracts;

namespace HimuOJ.Common.WebHostDefaults.Extensions;

public static class AuthenticationExtensions
{
    /// <summary>
    /// Add default authentication policy to service.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default authentication policy will configure the service as an API protected by Identity Server.
    /// </para>
    /// <para>
    /// The configuration within the service is as follows:
    /// <code>
    ///     "IdentityServer": {
    ///          "Url": "https://identity-api",
    ///          "Audience": "api",
    ///          "Scopes": {
    ///            "api": "API",
    ///            "the-scope-you-want": "The Scope You Want"
    ///          }
    ///     },
    /// </code>
    /// Audience must match the API Resources in the Identity Server configuration.
    /// </para>
    /// <para>
    /// By default, Issuers and Audience will be checked. 
    /// In debugging mode, the Require HttpsMetadata restriction will also be disabled.
    /// </para>
    /// </remarks>
    public static IHostApplicationBuilder AddDefaultAuthenticationPolicy(this IHostApplicationBuilder builder)
    {
        var identityServer = builder.Configuration.GetRequiredSection("IdentityServer");

        string identityServerUrl = identityServer.GetValue<string>("Url")
            ?? throw new ArgumentException("IdentityServer:Url is not configured");
        string audience = identityServer.GetValue<string>("Audience")
            ?? throw new ArgumentException("IdentityServer:Audience is not configured");

        Log.Information("Service {audience} using Identity Server at {url}", audience, identityServerUrl);

        builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
#if DEBUG
                options.RequireHttpsMetadata = false;
#endif
                options.Authority = identityServerUrl;
                options.TokenValidationParameters.ValidIssuers = [identityServerUrl];
                options.Audience = audience;
                options.TokenValidationParameters.ValidateAudience = false;
            });

        builder.Services.AddAuthorization();

        return builder;
    }
}
