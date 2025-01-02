using HimuOJ.Common.WebHostDefaults.Extensions;
using Identity.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                    foreach (string role in ApplicationRole.RolePermissionPriority.Keys)
                    {
                        await roleManager.CreateAsync(new ApplicationRole
                        {
                            Name = role,
                            NormalizedName = role.ToUpper()
                        });

                        logger.LogDebug("Role created: {RoleName}", role);
                    }
                }

                if (!await context.Users.AnyAsync())
                {
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    var (user, rawPassword) = GetInitialUserFromConfig(configuration);
                    var result = await userManager.CreateAsync(user, rawPassword);

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    logger.LogDebug("Initial user created: {UserName}", user.UserName);
                }

                await context.SaveChangesAsync();
            }
        }

        private static (ApplicationUser user, string rawPassword) GetInitialUserFromConfig(IConfiguration configuration)
        {
            var initialUserConfig = configuration.GetSection("InitialUser");
            var userName = initialUserConfig.GetValue<string>("Name");
            var password = initialUserConfig.GetValue<string>("Password");
            var mail = initialUserConfig.GetValue<string>("Mail");
            if (userName == null || password == null || mail == null)
                throw new ArgumentException("Invalid configuration in InitialUser");

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = mail
            };
            user.EmailConfirmed = true;

            return (user, password);
        }
    }
}
