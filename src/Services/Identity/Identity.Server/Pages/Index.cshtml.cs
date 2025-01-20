// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

#region

using System.Reflection;
using Duende.IdentityServer;
using Duende.IdentityServer.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

#endregion

namespace Identity.Server.Pages.Home
{
    [AllowAnonymous]
    public class Index : PageModel
    {
        public Index(IdentityServerLicense? license = null)
        {
            License = license;
        }

        public string Version
        {
            get => typeof(IdentityServerMiddleware).Assembly
                       .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                       ?.InformationalVersion.Split('+')
                       .First()
                   ?? "unavailable";
        }

        public IdentityServerLicense? License { get; }
    }
}