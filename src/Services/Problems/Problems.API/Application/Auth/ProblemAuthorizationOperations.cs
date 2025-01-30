using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace HimuOJ.Services.Problems.API.Application.Auth;

/// <summary>
/// Some specific authorization operations for the Problem entity.
/// </summary>
public static class ProblemAuthorizationOperations
{
    public static readonly OperationAuthorizationRequirement ReadInput 
        = new() { Name = nameof(ReadInput) };
    public static readonly OperationAuthorizationRequirement ReadExpectedOutput
        = new() { Name = nameof(ReadExpectedOutput) };
}
