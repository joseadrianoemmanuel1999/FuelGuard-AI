using FuelGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FuelGuard.Infrastructure.Persistence.Configurations;

public sealed class RiskScoreConfiguration : IEntityTypeConfiguration<RiskScore>
{
    public void Configure(EntityTypeBuilder<RiskScore> b)
    {
        b.HasKey(x => x.CompanyId);

        b.Property(x => x.Score).IsRequired();
        b.Property(x => x.Level).IsRequired();

        b.Property(x => x.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        b.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
