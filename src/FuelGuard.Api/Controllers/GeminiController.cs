using FuelGuard.Api.Contracts;
using FuelGuard.Application.Abstractions.Gemini;
using Microsoft.AspNetCore.Mvc;

namespace FuelGuard.Api.Controllers;

[ApiController]
[Route("api/gemini")]
public sealed class GeminiController(IGeminiService gemini) : ControllerBase
{
    [HttpPost("chat")]
    [ProducesResponseType(typeof(GeminiChatResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GeminiChatResponseDto>> ChatAsync(
        [FromBody] GeminiChatRequestDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest(new { error = "Question is required." });

        var result = await gemini.ChatAsync(
            new GeminiChatRequest(
                request.Question.Trim(),
                request.StationHint,
                request.PeriodHint,
                request.AdditionalContext),
            cancellationToken);

        return Ok(MapChat(result));
    }

    [HttpGet("insights")]
    [ProducesResponseType(typeof(GeminiInsightsResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<GeminiInsightsResponseDto>> GetInsightsAsync(CancellationToken cancellationToken)
    {
        var result = await gemini.GetInsightsAsync(cancellationToken);
        return Ok(MapInsights(result));
    }

    private static GeminiChatResponseDto MapChat(GeminiChatResponse r) =>
        new(r.Answer, r.RiskLevel, r.Recommendations, r.Explainability, r.PoweredByGemini, r.Disclaimer);

    private static GeminiInsightsResponseDto MapInsights(GeminiInsightsResponse r) =>
        new(
            r.OperationalInsights,
            r.AnomalyHighlights,
            r.Recommendations,
            r.ExecutiveSummary,
            r.RiskObservations,
            r.PoweredByGemini,
            r.Disclaimer);
}
