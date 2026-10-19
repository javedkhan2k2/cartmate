using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CartMate.Infrastructure.Persistence;

/// <summary>
/// Used only by the dotnet-ef tool (migrations add/script/bundle). It avoids booting the
/// whole API, which would need JWT keys, Sentry settings and more just to create a migration.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string LocalConnectionString =
        "Host=localhost;Port=5432;Database=cartmate;Username=cartmate;Password=cartmate_dev";

    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("CARTMATE_DESIGN_CONNECTION") ?? LocalConnectionString;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
