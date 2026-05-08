using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FuelGuard.Infrastructure.Persistence;

public sealed class FuelGuardDbContextFactory : IDesignTimeDbContextFactory<FuelGuardDbContext>
{
    public FuelGuardDbContext CreateDbContext(string[] args)
    {
        var raw = Environment.GetEnvironmentVariable("FUELGUARD_CONNECTION");
        if (string.IsNullOrWhiteSpace(raw))
        {
            var apiDir = ResolveApiProjectDirectory();
            if (string.IsNullOrEmpty(apiDir))
                throw new InvalidOperationException("Database connection string not configured.");

            raw = new ConfigurationBuilder()
                .SetBasePath(apiDir)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .AddUserSecrets(typeof(FuelGuardDbContextFactory).Assembly)
                .Build()
                .GetConnectionString("FuelGuard");
        }

        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidOperationException("Database connection string not configured.");

        var connectionString = SupabaseConnectionString.ForEntityFramework(raw);

        var options = new DbContextOptionsBuilder<FuelGuardDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new FuelGuardDbContext(options);
    }

    private static string? ResolveApiProjectDirectory()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "FuelGuard.Api.csproj")))
                return dir.FullName;

            var fromSrc = Path.Combine(dir.FullName, "src", "FuelGuard.Api", "FuelGuard.Api.csproj");
            if (File.Exists(fromSrc))
                return Path.GetDirectoryName(fromSrc);

            dir = dir.Parent;
        }

        return null;
    }
}
