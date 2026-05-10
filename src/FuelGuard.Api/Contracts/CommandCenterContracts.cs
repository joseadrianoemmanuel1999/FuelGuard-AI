namespace FuelGuard.Api.Contracts;

public sealed record NarrativeTokenDto(string Text, string Tone);

public sealed record RecommendedActionDto(string Title, string MaterialIcon, string Accent);

public sealed record AiInvestigationDto(
    IReadOnlyList<NarrativeTokenDto> SummaryTokens,
    IReadOnlyList<RecommendedActionDto> RecommendedActions);

public sealed record RiskAssessmentDto(
    int Score,
    string Level,
    string LevelLabel,
    string Summary);

public sealed record PipelineLogDto(
    string Timestamp,
    string Stage,
    string Message);

public sealed record AlertDto(
    string Id,
    string Title,
    string Severity,
    DateTimeOffset DetectedAt,
    string Detail);

public sealed record TelemetryDto(
    decimal FlowRateLitersPerSecond,
    decimal PressureBar,
    decimal TemperatureCelsius,
    string StatusLabel,
    string PanelTitle,
    string MapBadgeText);

public sealed record CommandCenterSimulationResultDto(
    RiskAssessmentDto Risk,
    AiInvestigationDto Investigation,
    IReadOnlyList<PipelineLogDto> PipelineLogs,
    IReadOnlyList<AlertDto> Alerts,
    TelemetryDto Telemetry,
    int ActivePipelineStageIndex,
    bool AlertStateActive);
