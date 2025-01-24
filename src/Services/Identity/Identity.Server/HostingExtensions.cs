﻿#region

using Common.BucketStorage.Minio;
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using HimuOJ.Common.BucketStorage;
using HimuOJ.Common.WebHostDefaults.Extensions;
using Identity.Server.Data;
using Identity.Server.Models;
using Identity.Server.Services;
using Microsoft.AspNetCore.Identity;
using Minio;
using Minio.Handlers;
using Serilog;

#endregion

namespace Identity.Server
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorPages();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);

                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                })
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
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    options.EmitStaticAudienceClaim = true;
                    options.KeyManagement.Enabled = true;

                    options.Discovery.CustomEntries.Add("local_api", "~/api");
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.GetClients(builder.Configuration))
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential();

            builder.Services.AddAuthentication();
            builder.Services.AddLocalApiAuthentication();
            builder.Services.AddControllers();

            builder.Services.AddTransient<IProfileService, ProfileService>();
            builder.Services.AddSingleton<UserResourcePathService>();

            builder.AddDefaultOpenApi();

            builder.Services.AddBucketStorage(builder.Configuration.GetSection("Storage"));

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDefaultOpenApi();
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