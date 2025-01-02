using Microsoft.EntityFrameworkCore.Design;

namespace HimuOJ.Services.Submits.Infrastructure;

// ReSharper disable once UnusedType.Global
public class SubmitsDbContextFactory : IDesignTimeDbContextFactory<SubmitsDbContext>
{
    public SubmitsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder   = new DbContextOptionsBuilder<SubmitsDbContext>();
        var connectionString = "Host=localhost;Database=HimuOJSubmitsDB;Username=postgres;Password=liuhuan123";
        optionsBuilder.UseNpgsql(connectionString);
        return new SubmitsDbContext(optionsBuilder.Options);
    }
}