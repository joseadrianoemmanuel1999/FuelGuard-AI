using FuelGuard.Api.Contracts;
using FuelGuard.Application.Abstractions;
using FuelGuard.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelGuard.Api.Controllers;

[ApiController]
[Route("api/anomalies")]
public sealed class AnomaliesController(IApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AnomalyResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AnomalyResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var items = await db.AnomalyDetections
            .AsNoTracking()
            .OrderByDescending(a => a.DetectedAt)
            .Select(a => new AnomalyResponse(
                a.Id,
                a.CompanyId,
                a.TankId,
                a.SnapshotDate,
                a.ExpectedValue,
                a.ActualValue,
                a.Severity,
                a.Explanation,
                a.DetectedAt))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }
}
