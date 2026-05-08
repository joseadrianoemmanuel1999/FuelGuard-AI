using FuelGuard.Domain.Enums;

namespace FuelGuard.Domain.Entities;

public class FuelTransaction
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid TankId { get; set; }
    public decimal QuantityLiters { get; set; }
    public FuelMovementType MovementType { get; set; }
    public DateTimeOffset OccurredAt { get; set; }

    public Company Company { get; set; } = null!;
    public FuelTank Tank { get; set; } = null!;
}
