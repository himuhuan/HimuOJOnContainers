#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

#endregion

namespace Identity.Server.Data
{
    public class IdentityDbContextDesignFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            var connectionString =
                "Host=localhost;Database=HimuOJIdentityDB;Username=postgres;Password=liuhuan123";
            optionsBuilder.UseNpgsql(connectionString);
            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}