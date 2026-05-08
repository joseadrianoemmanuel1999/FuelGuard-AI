using FuelGuard.Api.Contracts;
using FuelGuard.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelGuard.Api.Controllers;

[ApiController]
[Route("api/risk-scores")]
public sealed class RiskScoresController(IApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RiskScoreResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RiskScoreResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var items = await db.RiskScores
            .AsNoTracking()
            .Include(r => r.Company)
            .OrderBy(r => r.Company!.Name)
            .Select(r => new RiskScoreResponse(
                r.CompanyId,
                r.Company!.Name,
                r.Score,
                r.Level,
                r.Reason))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }
}
