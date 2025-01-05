using System.Collections;
using System.Data;
using System.Linq.Expressions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HimuOJ.Common.WebHostDefaults.Extensions
{
    public interface IDbContextSeeder<in TDbContext>
        where TDbContext : DbContext
    {
        Task SeedAsync(TDbContext context, IServiceProvider serviceProvider);
    }

    public static class DatabaseSupportExtensions
    {
        /// <summary>
        /// Add database connection for the specified DbContext.
        /// </summary>
        /// <remarks>
        /// The connection string in ConnectionStrings must be in the format of "DbContextName".
        /// For example, if the DbContext is named "IdentityDbContext", the key of connection string must be "IdentityDbContext".
        /// </remarks>
        /// <exception cref="ArgumentException" /> 
        public static IServiceCollection AddDatabaseConnection<TDbContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            bool usePooling = false)
            where TDbContext : DbContext
        {
            string contextName = typeof(TDbContext).Name;
            string connectionString = configuration.GetConnectionString(contextName)
                                      ?? throw new ArgumentException(
                                          $"Connection string for {contextName} is not found.");

            //services.AddDbContext<TDbContext>(options =>
            //{
            //    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            //           .EnableDetailedErrors()
            //           .EnableSensitiveDataLogging();
            //});

            // Now we use postgresql rather than mysql
            if (usePooling)
            {
                services.AddDbContextPool<TDbContext>(options => { options.UseNpgsql(connectionString); });
            }
            else
            {
                services.AddDbContext<TDbContext>(options => { options.UseNpgsql(connectionString); });
            }

            return services;
        }

        public static IServiceCollection AddDbContextMigration<TContext>(
            this IServiceCollection services,
            Type dbContextSeederType)
            where TContext : DbContext
        {
            services.AddScoped(typeof(IDbContextSeeder<TContext>), dbContextSeederType);
            var seeder = (TContext context, IServiceProvider serviceProvider) =>
            {
                var dbContextSeeder = serviceProvider.GetRequiredService<IDbContextSeeder<TContext>>();
                return dbContextSeeder.SeedAsync(context, serviceProvider);
            };
            return services.AddHostedService(sp => new MigrationHostedService<TContext>(sp, seeder));
        }

        public static IServiceCollection AddDbContextMigration<TContext, TDbContextSeeder>(
            this IServiceCollection services)
            where TContext : DbContext
            where TDbContextSeeder : IDbContextSeeder<TContext>
        {
            return services.AddDbContextMigration<TContext>(typeof(TDbContextSeeder));
        }

        public static IServiceCollection AddDbContextMigration<TContext>(
            this IServiceCollection services,
            Func<TContext, IServiceProvider, Task> seeder)
            where TContext : DbContext
        {
            return services.AddHostedService(sp => new MigrationHostedService<TContext>(sp, seeder));
        }

        public static IServiceCollection AddDbContextMigration<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            return services.AddDbContextMigration<TContext>((_, _) => Task.CompletedTask);
        }

        private static async Task MigrateDbContextAsync<TContext>(
            this IServiceProvider services,
            Func<TContext, IServiceProvider, Task> seeder)
            where TContext : DbContext
        {
            using var serviceScope    = services.CreateScope();
            var       serviceProvider = serviceScope.ServiceProvider;
            var       logger          = serviceProvider.GetRequiredService<ILogger<TContext>>();
            var       context         = serviceProvider.GetRequiredService<TContext>();

            try
            {
                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}",
                        typeof(TContext).Name);
                    await context.Database.MigrateAsync();
                    await seeder(context, serviceProvider);
                });
            }
            catch (Exception e)
            {
                logger.LogError(e, "FATAL: An exception occurred while migrating the context ({ContextName}) seed.",
                    context.GetGenericTypeName());
                throw;
            }
        }

        private class MigrationHostedService<TContext>(
            IServiceProvider serviceProvider,
            Func<TContext, IServiceProvider, Task> seeder
        )
            : BackgroundService where TContext : DbContext
        {
            public override Task StartAsync(CancellationToken cancellationToken)
            {
                return serviceProvider.MigrateDbContextAsync(seeder);
            }

            protected override Task ExecuteAsync(CancellationToken stoppingToken)
            {
                return Task.CompletedTask;
            }
        }

        public static string GetNpgsqlLocalConnectionString<TContext>(this IConfiguration configuration)
            where TContext : DbContext
        {
            string? connectionString = configuration.GetConnectionString(typeof(TContext).Name);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    $"from {nameof(TContext)}: Connection string for {typeof(TContext).Name} is not found.");
            }

            const string serverDeclTag = "Host=";
            int          sp            = connectionString.IndexOf(serverDeclTag);
            if (sp != -1)
            {
                int start = sp + serverDeclTag.Length;
                int end   = connectionString.IndexOf(';', start);
                int len   = end == -1 ? connectionString.Length - start : end - start;

                string hostToReplace = connectionString.Substring(start, len);
                connectionString = connectionString.Replace(hostToReplace, "localhost");
            }

            return connectionString;
        }

        #region LINQ Extensions

        /// <summary>
        /// WhereIf extension method for IQueryable.
        /// </summary>
        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> query,
            bool condition,
            Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        #endregion

        #region Dapper

        /// <summary>
        /// Executes a raw SQL query asynchronously using Dapper and returns the result as an IEnumerable of type T.
        /// </summary>
        /// <typeparam name="T">The type of the result elements.</typeparam>
        /// <param name="database">The DatabaseFacade instance to execute the query on.</param>
        /// <param name="commandText">The raw SQL query to execute.</param>
        /// <param name="param">The parameters to pass to the SQL query.</param>
        /// <param name="commandTimeout">The optional command timeout.</param>
        /// <param name="commandType">The optional command type.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an IEnumerable of type T.</returns>
        public static async Task<IEnumerable<T>> RawQueryAsync<T>(
            this DatabaseFacade database,
            string commandText,
            object param,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            var            cn  = database.GetDbConnection();
            IDbTransaction trn = database.CurrentTransaction?.GetDbTransaction()!;
            return await cn.QueryAsync<T>(commandText, param, trn, commandTimeout, commandType);
        }
        
        #nullable enable
        public static async Task<T?> FirstOrDefault<T>(this Task<IEnumerable<T>> source)
        {
            return (await source).FirstOrDefault();
        }
        #nullable restore

        #endregion
    }
}