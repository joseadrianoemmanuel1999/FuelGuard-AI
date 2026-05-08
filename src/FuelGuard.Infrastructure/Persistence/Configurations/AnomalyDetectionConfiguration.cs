using FuelGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FuelGuard.Infrastructure.Persistence.Configurations;

public sealed class AnomalyDetectionConfiguration : IEntityTypeConfiguration<AnomalyDetection>
{
    public void Configure(EntityTypeBuilder<AnomalyDetection> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Explanation)
            .HasMaxLength(500)
            .IsRequired();

        b.Property(x => x.ExpectedValue).IsRequired();
        b.Property(x => x.ActualValue).IsRequired();
        b.Property(x => x.Severity).IsRequired();
        b.Property(x => x.SnapshotDate).IsRequired();
        b.Property(x => x.DetectedAt).IsRequired();

        b.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Tank)
            .WithMany()
            .HasForeignKey(x => x.TankId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.CompanyId);
        b.HasIndex(x => x.TankId);
        b.HasIndex(x => x.DetectedAt);
        b.HasIndex(x => new { x.TankId, x.SnapshotDate });
    }
}
