#region

using HimuOJ.Common.WebHostDefaults.Infrastructure;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace HimuOJ.Common.WebApiComponents.Extensions;

public static class ResultApiWebExtensions
{
    public static IActionResult ToHttpApiResult<T>(this ApiResult<T> apiResult)
    {
        return apiResult.Code switch
        {
            ApiResultCode.Ok                => new OkObjectResult(apiResult.Result),
            ApiResultCode.ResourceNotExist  => new NotFoundResult(),
            ApiResultCode.BadAuthentication => new UnauthorizedResult(),
            ApiResultCode.BadAuthorization  => new ForbidResult(),
            ApiResultCode.BadRequest        => new BadRequestObjectResult(apiResult.Message),
            _                               => new StatusCodeResult(500),
        };
    }

    public static async Task<IActionResult> ToHttpApiResultAsync<T>(this Task<ApiResult<T>> apiTask)
    {
        return (await apiTask).ToHttpApiResult();
    }

    public static IActionResult ToHttpApiResult<T>(
        this T any,
        ApiResultCode code,
        string message = null)
    {
        return (new ApiResult<T>(any, code, message)).ToHttpApiResult();
    }

    public static ApiResult<T> ToApiResult<T>(this T any, ApiResultCode code, string message = null)
    {
        return new ApiResult<T>(any, code, message);
    }
}