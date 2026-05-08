using FuelGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuelGuard.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Company> Companies { get; }
    DbSet<FuelTank> FuelTanks { get; }
    DbSet<FuelTransaction> FuelTransactions { get; }
    DbSet<DailyFuelSnapshot> DailyFuelSnapshots { get; }
    DbSet<AnomalyDetection> AnomalyDetections { get; }
    DbSet<RiskScore> RiskScores { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
