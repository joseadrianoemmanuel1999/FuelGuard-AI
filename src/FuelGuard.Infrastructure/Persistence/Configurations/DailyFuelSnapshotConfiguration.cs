using FuelGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FuelGuard.Infrastructure.Persistence.Configurations;

public sealed class DailyFuelSnapshotConfiguration : IEntityTypeConfiguration<DailyFuelSnapshot>
{
    public void Configure(EntityTypeBuilder<DailyFuelSnapshot> b)
    {
        b.HasKey(x => new { x.TankId, x.Date });

        b.Property(x => x.OpeningStock).IsRequired();
        b.Property(x => x.ClosingStock).IsRequired();

        b.HasOne(x => x.Tank)
            .WithMany(x => x.DailySnapshots)
            .HasForeignKey(x => x.TankId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.Date);
    }
}
