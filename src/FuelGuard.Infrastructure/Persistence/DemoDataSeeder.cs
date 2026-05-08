using FuelGuard.Domain.Entities;
using FuelGuard.Domain.Enums;
using FuelGuard.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FuelGuard.Infrastructure.Persistence;

public sealed class DemoDataSeeder(ILogger<DemoDataSeeder> logger)
{
    public async Task SeedAsync(FuelGuardDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Companies.AnyAsync(cancellationToken))
            return;

        logger.LogInformation("Seeding demo companies, tanks, and baseline risk scores...");

        var distributorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var stationId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var tankDiesel = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var tankPremium = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        db.Companies.AddRange(
            new Company
            {
                Id = distributorId,
                Name = "Northern Fuel Distributors",
                Type = CompanyType.Distributor
            },
            new Company
            {
                Id = stationId,
                Name = "Lakeside Gas & Go",
                Type = CompanyType.GasStation
            });

        db.FuelTanks.AddRange(
            new FuelTank
            {
                Id = tankDiesel,
                CompanyId = stationId,
                CapacityLiters = 75_000,
                FuelType = "Diesel"
            },
            new FuelTank
            {
                Id = tankPremium,
                CompanyId = stationId,
                CapacityLiters = 50_000,
                FuelType = "Premium"
            });

        db.RiskScores.AddRange(
            new RiskScore
            {
                CompanyId = distributorId,
                Score = 5,
                Level = RiskLevel.Low,
                Reason = "Seed baseline: distributor profile."
            },
            new RiskScore
            {
                CompanyId = stationId,
                Score = 8,
                Level = RiskLevel.Low,
                Reason = "Seed baseline: retail station profile."
            });

        await db.SaveChangesAsync(cancellationToken);
    }
}
