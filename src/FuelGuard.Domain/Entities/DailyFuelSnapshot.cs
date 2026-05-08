namespace FuelGuard.Domain.Entities;

public class DailyFuelSnapshot
{
    public Guid TankId { get; set; }
    public DateOnly Date { get; set; }
    public decimal OpeningStock { get; set; }
    public decimal ClosingStock { get; set; }

    public FuelTank Tank { get; set; } = null!;
}
