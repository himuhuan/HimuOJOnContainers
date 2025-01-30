using System.Security.Claims;
using HimuOJ.Common.WebApiComponents.Authorization;
using HimuOJ.Common.WebHostDefaults.Infrastructure.Auth;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace HimuOJ.Services.Problems.API.Application.Auth;

public class ProblemAuthorizationCrudHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, Problem>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Problem resource)
    {
        Guid userId = new(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        bool isDistributor = context.User.IsInRole(ApplicationRoleConstants.Distributor)
            || context.User.IsInRole(ApplicationRoleConstants.Administrator);

        if (requirement.Name == AuthorizationOperations.Create.Name)
        {
            // only the distributor can create a problem.
            if (isDistributor)
                context.Succeed(requirement);
        }
        else if (userId == resource.DistributorId || context.User.IsInRole("Administrator"))
        {
            // The distributor or Administrator can do anything.
            context.Succeed(requirement);
        }
        else if (requirement.Name == ProblemAuthorizationOperations.ReadInput.Name
            && resource.GuestAccessLimit.AllowDownloadInput)
        {
            // The guest can read the input of the problem.
            context.Succeed(requirement);
        }
        else if (requirement.Name == ProblemAuthorizationOperations.ReadExpectedOutput.Name
            && resource.GuestAccessLimit.AllowDownloadOutput)
        {
            // The guest can read the output of the problem.
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}