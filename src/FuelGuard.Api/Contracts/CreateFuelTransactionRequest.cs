using System.ComponentModel.DataAnnotations;
using FuelGuard.Domain.Enums;

namespace FuelGuard.Api.Contracts;

public sealed class CreateFuelTransactionRequest
{
    [Required]
    public Guid CompanyId { get; init; }

    [Required]
    public Guid TankId { get; init; }

    [Range(0.01, double.MaxValue)]
    public decimal QuantityLiters { get; init; }

    [Required]
    public FuelMovementType MovementType { get; init; }

    public DateTimeOffset? OccurredAt { get; init; }
}
