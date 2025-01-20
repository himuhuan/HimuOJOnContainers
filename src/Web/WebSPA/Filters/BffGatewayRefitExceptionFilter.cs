#region

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Refit;

#endregion

namespace HimuOJ.Web.WebSPA.Filters;

/// <summary>
///     Handle Refit exception, and return a friendly message to the client.
/// </summary>
public class BffGatewayRefitExceptionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Do nothing
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is ApiException refitException)
        {
            context.ExceptionHandled = true;
            context.Result = new ObjectResult(new
            {
                refitException.RequestMessage.RequestUri,
                refitException.StatusCode,
                refitException.Message,
            })
            {
                StatusCode = (int) refitException.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}