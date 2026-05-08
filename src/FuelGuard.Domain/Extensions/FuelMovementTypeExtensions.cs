using FuelGuard.Domain.Enums;

namespace FuelGuard.Domain.Extensions;

public static class FuelMovementTypeExtensions
{
    public static decimal ToSignedLiters(this FuelMovementType movementType, decimal quantityLiters) =>
        movementType switch
        {
            FuelMovementType.Inbound => quantityLiters,
            FuelMovementType.Outbound => -quantityLiters,
            FuelMovementType.TransferIn => quantityLiters,
            FuelMovementType.TransferOut => -quantityLiters,
            _ => throw new ArgumentOutOfRangeException(nameof(movementType), movementType, null)
        };
}
