using FuelGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FuelGuard.Infrastructure.Persistence.Configurations;

public sealed class FuelTankConfiguration : IEntityTypeConfiguration<FuelTank>
{
    public void Configure(EntityTypeBuilder<FuelTank> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.FuelType)
            .HasMaxLength(64)
            .IsRequired();

        b.Property(x => x.CapacityLiters)
            .IsRequired();

        b.HasOne(x => x.Company)
            .WithMany(x => x.Tanks)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.CompanyId);
    }
}
