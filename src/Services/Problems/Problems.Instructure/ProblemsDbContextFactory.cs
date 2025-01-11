#region

using Microsoft.EntityFrameworkCore.Design;

#endregion

namespace HimuOJ.Services.Problems.Infrastructure;

internal class ProblemsDbContextFactory : IDesignTimeDbContextFactory<ProblemsDbContext>
{
    public ProblemsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProblemsDbContext>();
        var connectionString =
            "Host=localhost;Database=HimuOJProblemsDB;Username=postgres;Password=liuhuan123";
        optionsBuilder.UseNpgsql(connectionString);
        return new ProblemsDbContext(optionsBuilder.Options);
    }
}