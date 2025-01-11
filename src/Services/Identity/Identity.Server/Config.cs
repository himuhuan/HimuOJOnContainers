#region

using Duende.IdentityServer;
using Duende.IdentityServer.Models;

#endregion

namespace Identity.Server
{
    public static class Config
    {
        public static IEnumerable<ApiResource> Apis =>
        [
            new ApiResource("problems", "Problems API")
            {
                Scopes = { "problems" }
            },

            new ApiResource("submits", "Submits API")
            {
                Scopes = { "submits" }
            }
        ];

        public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope("problems", displayName: "Problems API"),
            new ApiScope("submits", displayName: "Submits API")
        ];

        public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        ];

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            return
            [
                new Client
                {
                    ClientId      = "webspa",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { $"{configuration["WebSpaClient"]}/signin-oidc" },
                    PostLogoutRedirectUris =
                        { $"{configuration["WebSpaClient"]}/signout-callback-oidc" },

                    AllowOfflineAccess               = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes =
                    [
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "problems",
                        "submits"
                    ]
                },

                new Client
                {
                    ClientId      = "problemsswaggerui",
                    ClientName    = "Problems Swagger UI",
                    ClientSecrets = { new Secret("problems-secret".Sha256()) },

                    AllowedGrantTypes           = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        $"{configuration["ProblemsApiExternalUrl"]}/api-reference/v1"
                    },
                    PostLogoutRedirectUris =
                        { $"{configuration["ProblemsApiExternalUrl"]}/api-reference/v1" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "problems"
                    }
                },

                new Client
                {
                    ClientId      = "submitsswaggerui",
                    ClientName    = "Submits Swagger UI",
                    ClientSecrets = { new Secret("submits-secret".Sha256()) },

                    AllowedGrantTypes           = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        $"{configuration["SubmitsApiExternalUrl"]}/api-reference/v1"
                    },
                    PostLogoutRedirectUris =
                        { $"{configuration["SubmitsApiExternalUrl"]}/api-reference/v1" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "submits"
                    }
                }
            ];
        }
    }
}