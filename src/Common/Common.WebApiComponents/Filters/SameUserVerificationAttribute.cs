using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HimuOJ.Common.WebApiComponents.Filters;

/// <summary>
/// An attribute that verifies if the user making the request is the same
/// as the user specified in the request body.
/// </summary>
public class SameUserVerificationAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        string tokenUserId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        string bodyUserId  = context.ActionArguments["userId"]?.ToString();

        if (bodyUserId == null)
        {
            context.Result = new BadRequestObjectResult("userId is required in the request body.");
            return;
        }
        
        if (tokenUserId != bodyUserId)
        {
            context.Result = new ForbidResult();
            return;
        }
        
        await next();
    }
}