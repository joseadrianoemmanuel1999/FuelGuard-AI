using FuelGuard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FuelGuard.Application.UnitTests.Support;

internal static class TestDb
{
    public static FuelGuardDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<FuelGuardDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new FuelGuardDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
