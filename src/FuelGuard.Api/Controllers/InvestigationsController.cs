using FuelGuard.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FuelGuard.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class InvestigationsController : ControllerBase
{
    [HttpGet("investigations")]
    [ProducesResponseType(typeof(InvestigationsBoardDto), StatusCodes.Status200OK)]
    public ActionResult<InvestigationsBoardDto> GetInvestigations() =>
        Ok(PortalStaticContent.InvestigationsBoard);
}
