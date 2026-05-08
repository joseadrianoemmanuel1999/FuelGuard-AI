namespace FuelGuard.Domain.Entities;

public class FuelTank
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public decimal CapacityLiters { get; set; }
    public string FuelType { get; set; } = string.Empty;

    public Company Company { get; set; } = null!;
    public ICollection<FuelTransaction> Transactions { get; set; } = new List<FuelTransaction>();
    public ICollection<DailyFuelSnapshot> DailySnapshots { get; set; } = new List<DailyFuelSnapshot>();
}
