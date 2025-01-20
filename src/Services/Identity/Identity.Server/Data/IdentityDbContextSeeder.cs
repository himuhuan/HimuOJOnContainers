#region

using HimuOJ.Common.WebHostDefaults.Extensions;
using Identity.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Identity.Server.Data
{
    public class IdentityDbContextSeeder : IDbContextSeeder<IdentityDbContext>
    {
        public async Task SeedAsync(IdentityDbContext context, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var logger = serviceProvider.GetRequiredService<ILogger<IdentityDbContextSeeder>>();

            using (context)
            {
                if (!await context.Roles.AnyAsync())
                {
                    foreach (var role in ApplicationRole.GetAllRoles())
                    {
                        await roleManager.CreateAsync(role);
                        logger.LogDebug("Role created: {@Role}", role);
                    }
                }

                if (!await context.Users.AnyAsync())
                {
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    (ApplicationUser user, string rawPassword) =
                        GetInitialUserFromConfig(configuration);
                    var result = await userManager.CreateAsync(user, rawPassword);

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    logger.LogDebug("Initial user created: {UserName}", user.UserName);
                    await userManager.AddToRolesAsync(
                        user, ApplicationRole.GetAllRoles().Select(r => r.Name!));
                }

                await context.SaveChangesAsync();
            }
        }

        private static (ApplicationUser user, string rawPassword) GetInitialUserFromConfig(
            IConfiguration configuration)
        {
            var initialUserConfig = configuration.GetSection("InitialUser");
            var userName = initialUserConfig.GetValue<string>("Name");
            var password = initialUserConfig.GetValue<string>("Password");
            var mail = initialUserConfig.GetValue<string>("Mail");
            if (userName == null || password == null || mail == null)
                throw new ArgumentException("Invalid configuration in InitialUser");

            var user = new ApplicationUser
            {
                // TODO: may be better to use a random GUID in production?
                Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0).ToString(),
                UserName = userName,
                Email = mail,
                EmailConfirmed = true
            };

            return (user, password);
        }
    }
}