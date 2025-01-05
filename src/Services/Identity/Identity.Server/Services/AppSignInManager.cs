using Identity.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Server.Services;

public class AppSignInManager : SignInManager<ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public AppSignInManager(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<ApplicationUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<ApplicationUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        _userManager = userManager;
    }

    protected override async Task<SignInResult> SignInOrTwoFactorAsync(
        ApplicationUser user,
        bool isPersistent,
        string? loginProvider = null,
        bool bypassTwoFactor = false)
    {
        user.LastLoginDate = DateOnly.FromDateTime(DateTime.UtcNow);
        await _userManager.UpdateAsync(user);
        return await base.SignInOrTwoFactorAsync(user, isPersistent, loginProvider, bypassTwoFactor);
    }
}