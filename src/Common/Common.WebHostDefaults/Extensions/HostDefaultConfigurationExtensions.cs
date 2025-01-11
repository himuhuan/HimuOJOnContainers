#region

using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;

#endregion

namespace HimuOJ.Common.WebHostDefaults.Extensions;

public static class HostDefaultConfigurationExtensions
{
    public static WebApplicationBuilder ConfigureDefaultPorts(this WebApplicationBuilder builder)
    {
        int httpPort  = builder.Configuration.GetValue("ASPNETCORE_HTTP_PORTS", 80);
        int httpsPort = builder.Configuration.GetValue("ASPNETCORE_HTTPS_PORTS", 81);

        builder.WebHost.ConfigureKestrel((context, options) =>
        {
            options.Listen(IPAddress.Any, httpPort,
                httpOpt => { httpOpt.Protocols = HttpProtocols.Http1AndHttp2; });
            options.Listen(IPAddress.Any, httpsPort,
                listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
        });

        return builder;
    }
}