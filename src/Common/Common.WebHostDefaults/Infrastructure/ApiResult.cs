using System.Text.Json.Serialization;

namespace HimuOJ.Common.WebHostDefaults.Infrastructure;

/// <summary>
/// Our API result model
/// </summary>
/// <remarks>
/// Our API always returns an error code instead of an exception when a non-fatal error occurs. 
/// If the error is caused by code or external operation failure, an exception will be thrown - at this point, we are powerless.
/// </remarks>
public record ApiResult<T>(T Result, ApiResultCode Code, string Message)
{
    [JsonIgnore] public bool IsSuccess => Code == ApiResultCode.Ok;

    public static ApiResult<T> Success(T result) => new(result, ApiResultCode.Ok, null);

    public static ApiResult<T> Success(T result, string message) => new(result, ApiResultCode.Ok, message);

    public static ApiResult<T> Error(ApiResultCode code) => new(default, code, null);
    
    public static ApiResult<T> Error(ApiResultCode code, string message) => new(default, code, message);
}
