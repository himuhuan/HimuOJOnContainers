namespace HimuOJ.Common.WebHostDefaults.Infrastructure;

public enum ApiResultCode
{
    Ok,

    /// <summary>
    ///     UnexpectedError means unforeseen results, often errors that logically hardly occur.
    ///     If such an error occurs, it is most likely an error in the code itself
    /// </summary>
    UnexpectedError,

    BadRequest,
    BadAuthentication,
    BadAuthorization,
    UserNotActivated,
    LockedUser,
    ResourceNotExist,
    DbOperationFailed,
    DuplicateItem,
    StorageFailed,

    InvalidData,

    /// <summary>
    ///     OutOfLimit means that some object in request exceeds the limit.
    /// </summary>
    OutOfLimit
}