using FuelGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FuelGuard.Infrastructure.Persistence.Configurations;

public sealed class FuelTransactionConfiguration : IEntityTypeConfiguration<FuelTransaction>
{
    public void Configure(EntityTypeBuilder<FuelTransaction> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.QuantityLiters)
            .IsRequired();

        b.Property(x => x.MovementType)
            .IsRequired();

        b.Property(x => x.OccurredAt)
            .IsRequired();

        b.HasOne(x => x.Company)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Tank)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.TankId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => new { x.TankId, x.OccurredAt });

        b.HasIndex(x => x.CompanyId);
    }
}
