using FuelGuard.Domain.Enums;

namespace FuelGuard.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CompanyType Type { get; set; }

    public ICollection<FuelTank> Tanks { get; set; } = new List<FuelTank>();
    public ICollection<FuelTransaction> Transactions { get; set; } = new List<FuelTransaction>();
}
