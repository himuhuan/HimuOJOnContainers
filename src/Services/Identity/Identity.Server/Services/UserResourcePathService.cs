using HimuOJ.Common.BucketStorage;
using Microsoft.Extensions.Options;

namespace Identity.Server.Services;

public class UserResourcePathService
{
    private readonly BucketStorageOptions _options;

    public UserResourcePathService(IOptions<BucketStorageOptions> options)
    {
        _options = options.Value;
    }

    public string GetUserAvatarPath(string userId, string avatarFileName)
    {
        return Path.Combine(_options.BucketName, "avatars", userId, avatarFileName);
    }
}
