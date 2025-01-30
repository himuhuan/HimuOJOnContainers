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
            ApiResultCode.Ok => new OkObjectResult(apiResult.Result),
            ApiResultCode.ResourceNotExist => new NotFoundObjectResult(apiResult),
            ApiResultCode.BadAuthentication => new UnauthorizedObjectResult(apiResult),
            ApiResultCode.BadAuthorization => new ForbidResult(),
            ApiResultCode.BadRequest => new BadRequestObjectResult(apiResult),
            _ => new ObjectResult(apiResult) { StatusCode = 500 },
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

    public static IActionResult ToHttpApiResult(this ApiResultCode code)
    {
        return (new ApiResult<ApiResultCode>(code, code, code.ToString())).ToHttpApiResult();
    }

    public static IActionResult ToHttpApiResult<T>(this ApiResultCode code, T value, string message = null)
    {
        return (new ApiResult<T>(value, code, message ?? code.ToString())).ToHttpApiResult();
    }

    public static IActionResult ToHttpApiResult(this ApiResultCode code, string message)
    {
        return (new ApiResult<ApiResultCode>(code, code, message)).ToHttpApiResult();
    }

    public static ApiResult<T> ToApiResult<T>(this T any, ApiResultCode code, string message = null)
    {
        return new ApiResult<T>(any, code, message);
    }
}