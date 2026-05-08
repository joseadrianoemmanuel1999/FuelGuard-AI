using FuelGuard.Application.Abstractions;
using FuelGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuelGuard.Infrastructure.Persistence;

public sealed class FuelGuardDbContext(DbContextOptions<FuelGuardDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<FuelTank> FuelTanks => Set<FuelTank>();
    public DbSet<FuelTransaction> FuelTransactions => Set<FuelTransaction>();
    public DbSet<DailyFuelSnapshot> DailyFuelSnapshots => Set<DailyFuelSnapshot>();
    public DbSet<AnomalyDetection> AnomalyDetections => Set<AnomalyDetection>();
    public DbSet<RiskScore> RiskScores => Set<RiskScore>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FuelGuardDbContext).Assembly);
    }
}
