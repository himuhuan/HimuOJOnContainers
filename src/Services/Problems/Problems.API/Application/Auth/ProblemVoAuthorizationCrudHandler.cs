using System.Security.Claims;
using HimuOJ.Common.WebApiComponents.Authorization;
using HimuOJ.Services.Problems.API.Application.Models.Vo;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace HimuOJ.Services.Problems.API.Application.Auth;

public class ProblemVoAuthorizationCrudHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, ProblemVo>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        ProblemVo resource)
    {
        if (requirement.Name != AuthorizationOperations.Read.Name)
        {
            // VO is immutable, so no need to check for other operations
            return Task.CompletedTask;
        }
        
        // The distributor or Administrator can read it.
        Guid userId = new(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (userId == resource.DistributorId || context.User.IsInRole("Administrator"))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}