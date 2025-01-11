#region

using Duende.IdentityServer.Services;
using HimuOJ.Common.WebHostDefaults.Extensions;
using Identity.Server.Data;
using Identity.Server.Models;
using Identity.Server.Services;
using Microsoft.AspNetCore.Identity;
using Serilog;

#endregion

namespace Identity.Server
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorPages();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddSignInManager<AppSignInManager>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            Log.Information("Applying seeder migration for {type}", nameof(IdentityDbContext));
            builder.Services.AddDbContextMigration<IdentityDbContext, IdentityDbContextSeeder>();

            Log.Information("Adding database connection for {type}", nameof(IdentityDbContext));
            builder.Services.AddDatabaseConnection<IdentityDbContext>(builder.Configuration);

            builder.Services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents       = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents     = true;
                    options.Events.RaiseSuccessEvents     = true;

                    options.EmitStaticAudienceClaim = true;
                    options.KeyManagement.Enabled   = true;
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.GetClients(builder.Configuration))
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential();

            builder.Services.AddAuthentication();

            builder.Services.AddTransient<IProfileService, ProfileService>();

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax
            });

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapDefaultControllerRoute();

            app.MapRazorPages()
                .RequireAuthorization();

            return app;
        }
    }
}