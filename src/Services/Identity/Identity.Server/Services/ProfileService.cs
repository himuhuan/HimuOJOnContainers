#region

using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Identity.Server.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;

#endregion

namespace Identity.Server.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserResourcePathService _userResourcePath;

        public ProfileService(
            UserManager<ApplicationUser> userManager,
            UserResourcePathService userResourcePath)
        {
            _userManager = userManager;
            _userResourcePath = userResourcePath;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject
                          ?? throw new ArgumentNullException(nameof(context.Subject));
            var subjectId = (subject.Claims.FirstOrDefault(x => x.Type == "sub")?.Value)
                            ?? throw new ArgumentException("subjectId is null");
            var user = await _userManager.FindByIdAsync(subjectId)
                       ?? throw new ArgumentException("Invalid subject identifier");
            var claims = GetClaims(user);
            context.IssuedClaims = claims.ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject
                          ?? throw new ArgumentNullException(nameof(context.Subject));
            var subjectId = (subject.Claims.FirstOrDefault(x => x.Type == "sub")?.Value)
                            ?? throw new ArgumentException("subjectId is null");
            var user = await _userManager.FindByIdAsync(subjectId);
            context.IsActive = false;
            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims
                        .Where(c => c.Type == "security_stamp")
                        .Select(c => c.Value)
                        .SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                            return;
                    }
                }

                context.IsActive = !user.LockoutEnabled
                                   || !user.LockoutEnd.HasValue
                                   || user.LockoutEnd <= DateTime.UtcNow;
            }
        }

        private List<Claim> GetClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new(JwtClaimTypes.Subject, user.Subject, ClaimValueTypes.Integer64),
                new(JwtClaimTypes.PreferredUserName, user.UserName ?? "null"),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "null")
            };

            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                claims.Add(new Claim(
                    "avatar",
                    _userResourcePath.GetUserAvatarFullPath(user.Id, user.Avatar))
                );
            }

            if (!string.IsNullOrWhiteSpace(user.Background))
                claims.Add(new Claim("background", user.Background));

            if (_userManager.SupportsUserEmail)
            {
                claims.AddRange(
                [
                    new Claim(JwtClaimTypes.Email, user.Email ?? "null"),
                    new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false",
                        ClaimValueTypes.Boolean)
                ]);
            }

            if (_userManager.SupportsUserPhoneNumber
                && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.AddRange(
                [
                    new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                    new Claim(JwtClaimTypes.PhoneNumberVerified,
                        user.PhoneNumberConfirmed ? "true" : "false",
                        ClaimValueTypes.Boolean)
                ]);
            }

            _userManager.GetRolesAsync(user).Result.ToList().ForEach(role =>
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            });

            return claims;
        }
    }
}