using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace HimuOJ.Common.WebApiComponents.Authorization;

/// <summary>
/// Provides a set of authorization operations for the application.
/// </summary>
public static class AuthorizationOperations
{
    public static readonly OperationAuthorizationRequirement Read = new()
        { Name = nameof(Read) };

    public static readonly OperationAuthorizationRequirement Create = new()
        { Name = nameof(Create) };

    public static readonly OperationAuthorizationRequirement Update = new()
        { Name = nameof(Update) };

    public static readonly OperationAuthorizationRequirement Delete = new()
        { Name = nameof(Delete) };
}