#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

#endregion

namespace HimuOJ.Common.WebHostDefaults.Infrastructure.OpenApi;

public class AuthenticationOperationFilter(string[] RequiredScopes) : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var requireAuthorization = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .OfType<IAuthorizeData>()
            .Any();

        if (!requireAuthorization)
        {
            return;
        }

        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403",
            new OpenApiResponse { Description = "Permission denied" });

        var oAuthScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id   = "oauth2"
            }
        };

        operation.Security =
        [
            new OpenApiSecurityRequirement
            {
                [oAuthScheme] = RequiredScopes
            }
        ];
    }
}