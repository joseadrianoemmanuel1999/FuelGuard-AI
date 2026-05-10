using FuelGuard.Api.Contracts;
using FuelGuard.Application.Abstractions;
using FuelGuard.Application.FuelTransactions;
using FuelGuard.Domain.Enums;
using FuelGuard.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelGuard.Api.Controllers;

[ApiController]
[Route("api/command-center")]
public sealed class CommandCenterController(
    IApplicationDbContext db,
    CreateFuelTransactionHandler transactionHandler) : ControllerBase
{
    private static readonly Guid DemoStationId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid DemoTankId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [HttpGet("current-risk")]
    [ProducesResponseType(typeof(RiskAssessmentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RiskAssessmentDto>> GetCurrentRiskAsync(CancellationToken cancellationToken) =>
        Ok(await BuildRiskAsync(cancellationToken));

    [HttpGet("pipeline-logs")]
    [ProducesResponseType(typeof(IReadOnlyList<PipelineLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PipelineLogDto>>> GetPipelineLogsAsync(CancellationToken cancellationToken) =>
        Ok(await BuildPipelineLogsAsync(cancellationToken, includeSimulationTail: false));

    [HttpGet("alerts")]
    [ProducesResponseType(typeof(IReadOnlyList<AlertDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AlertDto>>> GetAlertsAsync(CancellationToken cancellationToken) =>
        Ok(await BuildAlertsAsync(cancellationToken));

    [HttpGet("telemetry")]
    [ProducesResponseType(typeof(TelemetryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TelemetryDto>> GetTelemetryAsync(CancellationToken cancellationToken) =>
        Ok(await BuildTelemetryAsync(cancellationToken, stressed: false));

    [HttpGet("ai-investigation")]
    [ProducesResponseType(typeof(AiInvestigationDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AiInvestigationDto>> GetAiInvestigationAsync(CancellationToken cancellationToken) =>
        Ok(await BuildInvestigationAsync(cancellationToken));

    [HttpPost("simulate-suspicious-activity")]
    [ProducesResponseType(typeof(CommandCenterSimulationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<CommandCenterSimulationResultDto>> SimulateSuspiciousActivityAsync(
        CancellationToken cancellationToken) =>
        RunSimulationAsync(cancellationToken);

    [HttpPost("simulate-fuel-smuggling")]
    [ProducesResponseType(typeof(CommandCenterSimulationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<CommandCenterSimulationResultDto>> SimulateFuelSmugglingAsync(
        CancellationToken cancellationToken) =>
        RunSimulationAsync(cancellationToken);

    [HttpGet("live-feed")]
    [ProducesResponseType(typeof(IReadOnlyList<LiveFeedEventDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<LiveFeedEventDto>> GetLiveFeed() =>
        Ok(PortalStaticContent.LiveFeed);

    [HttpGet("investigations")]
    [ProducesResponseType(typeof(InvestigationsBoardDto), StatusCodes.Status200OK)]
    public ActionResult<InvestigationsBoardDto> GetInvestigationsBoard() =>
        Ok(PortalStaticContent.InvestigationsBoard);

    [HttpGet("reports")]
    [ProducesResponseType(typeof(ExecutiveRiskReportDto), StatusCodes.Status200OK)]
    public ActionResult<ExecutiveRiskReportDto> GetExecutiveReports() =>
        Ok(PortalStaticContent.ExecutiveReport);

    [HttpGet("team")]
    [ProducesResponseType(typeof(TeamDirectoryDto), StatusCodes.Status200OK)]
    public ActionResult<TeamDirectoryDto> GetTeamDirectory() =>
        Ok(PortalStaticContent.TeamDirectory);

    [HttpGet("terminal-lines")]
    [ProducesResponseType(typeof(IReadOnlyList<TerminalLogLineDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<TerminalLogLineDto>> GetTerminalLines() =>
        Ok(PortalStaticContent.TerminalLines);

    private async Task<ActionResult<CommandCenterSimulationResultDto>> RunSimulationAsync(
        CancellationToken cancellationToken)
    {
        var cmd = new CreateFuelTransactionCommand(
            DemoStationId,
            DemoTankId,
            18_000,
            FuelMovementType.Outbound,
            DateTimeOffset.UtcNow);

        try
        {
            await transactionHandler.HandleAsync(cmd, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        await Task.Delay(200, cancellationToken);

        var risk = await BuildRiskAsync(cancellationToken);
        var investigation = await BuildInvestigationAsync(cancellationToken);
        var logs = await BuildPipelineLogsAsync(cancellationToken, includeSimulationTail: true);
        var alerts = await BuildAlertsAsync(cancellationToken);
        var telemetry = await BuildTelemetryAsync(cancellationToken, stressed: true);

        return Ok(new CommandCenterSimulationResultDto(
            risk,
            investigation,
            logs,
            alerts,
            telemetry,
            ActivePipelineStageIndex: 4,
            AlertStateActive: true));
    }

    private async Task<RiskAssessmentDto> BuildRiskAsync(CancellationToken cancellationToken)
    {
        var row = await db.RiskScores
            .AsNoTracking()
            .OrderByDescending(r => r.Score)
            .Select(r => new { r.Score, r.Level, r.Reason })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
            return new RiskAssessmentDto(0, "LOW", "Risk Level: LOW", "No risk data seeded.");

        var level = MapLevel(row.Level);
        var score = (int)Math.Clamp(Math.Round(row.Score, MidpointRounding.AwayFromZero), 0, 100);
        return new RiskAssessmentDto(
            score,
            level,
            $"Risk Level: {level}",
            row.Reason);
    }

    private static string MapLevel(RiskLevel level) => level switch
    {
        RiskLevel.Low => "LOW",
        RiskLevel.Medium => "MEDIUM",
        RiskLevel.High => "HIGH",
        RiskLevel.Critical => "CRITICAL",
        _ => "UNKNOWN"
    };

    private async Task<AiInvestigationDto> BuildInvestigationAsync(CancellationToken cancellationToken)
    {
        var latest = await db.AnomalyDetections
            .AsNoTracking()
            .OrderByDescending(a => a.DetectedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest is not null)
        {
            var delta = Math.Abs(latest.ActualValue - latest.ExpectedValue);
            var tokens = new List<NarrativeTokenDto>
            {
                new("Anomaly detected in ", "default"),
                new("Southern Sector", "primary"),
                new(" pipeline flow. Volume variance exceeds ", "default"),
                new($"{delta:F1}% threshold", "error"),
                new(". Patterns suggest ", "default"),
                new("illegal siphoning", "error"),
                new($" at Node 42. High confidence match with known smuggling vectors. ({latest.Explanation})", "default")
            };

            return new AiInvestigationDto(tokens, DefaultRecommendedActions());
        }

        return new AiInvestigationDto(
            new[]
            {
                new NarrativeTokenDto(
                    "\"Telemetry nominal across monitored nodes. AI models scanning aggregate flow for siphoning signatures. No active investigation threads.\"",
                    "default")
            },
            DefaultRecommendedActions());
    }

    private static IReadOnlyList<RecommendedActionDto> DefaultRecommendedActions() =>
    [
        new RecommendedActionDto("Shut down Valve 42", "dangerous", "error"),
        new RecommendedActionDto("Deploy Inspection Drone", "flight_takeoff", "tertiary"),
        new RecommendedActionDto("Notify Local Compliance", "assignment_ind", "primary")
    ];

    private async Task<IReadOnlyList<PipelineLogDto>> BuildPipelineLogsAsync(
        CancellationToken cancellationToken,
        bool includeSimulationTail)
    {
        var now = DateTimeOffset.Now;
        var logs = new List<PipelineLogDto>
        {
            new(now.AddSeconds(-120).ToString("HH:mm:ss"), "INGESTION", "Data stream initialized from 14 nodes"),
            new(now.AddSeconds(-118).ToString("HH:mm:ss"), "PROTOCOL", "Handshake verified for Edge-Gateway-04")
        };

        var lastAnomaly = await db.AnomalyDetections
            .AsNoTracking()
            .OrderByDescending(a => a.DetectedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastAnomaly is not null)
        {
            var t = lastAnomaly.DetectedAt.ToLocalTime().ToString("HH:mm:ss");
            var delta = Math.Abs(lastAnomaly.ActualValue - lastAnomaly.ExpectedValue);
            logs.Add(new PipelineLogDto(t, "ANOMALY", $"Deviation detected in Node 42 (Delta: +{delta:F1}%)"));
            logs.Add(new PipelineLogDto(t, "RISK", "Escalating to Level 3 Internal Investigation"));
        }

        logs.Add(new PipelineLogDto(now.AddSeconds(-108).ToString("HH:mm:ss"), "AI_CORE", "Cross-referencing historical siphoning patterns..."));
        logs.Add(new PipelineLogDto(now.AddSeconds(-104).ToString("HH:mm:ss"), "AI_CORE", "Match found: Sector Southern-7 Pattern Beta-4"));
        logs.Add(new PipelineLogDto(now.AddSeconds(-100).ToString("HH:mm:ss"), "SYSTEM", "Visual feed from Drone-A9 engaged"));

        if (includeSimulationTail)
        {
            var t = now.ToString("HH:mm:ss");
            logs.Add(new PipelineLogDto(t, "INGESTION", "Spike transaction ingested — multi-agent correlation started"));
            logs.Add(new PipelineLogDto(t, "AI_CORE", "Qwen classifier: high-confidence siphoning vector (live replay)"));
            logs.Add(new PipelineLogDto(t, "ALERT", "Security personnel dispatched to Node 42 coordinates"));
        }
        else
            logs.Add(new PipelineLogDto(now.AddSeconds(-96).ToString("HH:mm:ss"), "ALERT", "Security personnel dispatched to Node 42 coordinates"));

        return logs;
    }

    private async Task<IReadOnlyList<AlertDto>> BuildAlertsAsync(CancellationToken cancellationToken) =>
        await db.AnomalyDetections
            .AsNoTracking()
            .OrderByDescending(a => a.DetectedAt)
            .Take(8)
            .Select(a => new AlertDto(
                a.Id.ToString(),
                "ACTIVE ALERT: SECTOR 7-G",
                a.Severity.ToString(),
                a.DetectedAt,
                a.Explanation))
            .ToListAsync(cancellationToken);

    private static Task<TelemetryDto> BuildTelemetryAsync(CancellationToken cancellationToken, bool stressed)
    {
        _ = cancellationToken;
        if (stressed)
            return Task.FromResult(new TelemetryDto(
                412.5m,
                14.2m,
                24.5m,
                "CRITICAL DEVIATION",
                "Node 42 Telemetry",
                "ACTIVE ALERT: SECTOR 7-G"));

        return Task.FromResult(new TelemetryDto(
            398.1m,
            12.1m,
            24.2m,
            "STABLE",
            "Node 42 Telemetry",
            "MONITORING"));
    }
}
