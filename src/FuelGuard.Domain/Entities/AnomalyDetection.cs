using FuelGuard.Shared.Models;

namespace FuelGuard.Domain.Entities;

public class AnomalyDetection
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid TankId { get; set; }
    public DateOnly SnapshotDate { get; set; }
    public decimal ExpectedValue { get; set; }
    public decimal ActualValue { get; set; }
    public AnomalySeverity Severity { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public DateTimeOffset DetectedAt { get; set; } = DateTimeOffset.UtcNow;

    public Company Company { get; set; } = null!;
    public FuelTank Tank { get; set; } = null!;
}
