using FuelGuard.Shared.Models;

namespace FuelGuard.Domain.Entities;

public class RiskScore
{
    public Guid CompanyId { get; set; }
    public decimal Score { get; set; }
    public RiskLevel Level { get; set; }
    public string Reason { get; set; } = string.Empty;

    public Company Company { get; set; } = null!;
}
