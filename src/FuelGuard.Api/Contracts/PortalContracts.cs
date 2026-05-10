namespace FuelGuard.Api.Contracts;

public sealed record LiveFeedEventDto(
    string Timestamp,
    string Category,
    string Title,
    string Body,
    IReadOnlyList<string> Tags,
    string Accent);

public sealed record InvestigationBoardRowDto(
    string CompanyName,
    string InvestigationId,
    string RiskLabel,
    string RiskTone,
    int ConfidencePercent,
    string Region,
    string Status,
    bool StatusActiveDot,
    string LastActivity);

public sealed record InvestigationTimelineItemDto(string TimeUtc, string Title, string DotTone);

public sealed record InvestigationDetailDto(
    string CaseId,
    string NarrativeHtml,
    IReadOnlyList<InvestigationTimelineItemDto> Timeline,
    decimal NormalFlowLitersPerSecond,
    decimal ActualFlowLitersPerSecond);

public sealed record InvestigationsBoardDto(
    IReadOnlyList<InvestigationBoardRowDto> Rows,
    InvestigationDetailDto SelectedDetail);

public sealed record ExecutiveSummaryCardDto(
    string Label,
    string Value,
    string Subtext,
    string BorderTone,
    string IconName);

public sealed record PipelineSectorDto(
    string Code,
    string StatusLabel,
    string StatusTone,
    string Description,
    decimal RiskIndex,
    string FooterLabel,
    string FooterTone);

public sealed record ExecutiveRiskReportDto(
    IReadOnlyList<ExecutiveSummaryCardDto> SummaryCards,
    IReadOnlyList<PipelineSectorDto> Sectors,
    string MapImageUrl,
    string ChartAnnotation);

public sealed record TeamMemberDto(
    string Name,
    string Role,
    int Level,
    string Status,
    string Assignment,
    string AvatarUrl,
    bool OnlineDot,
    IReadOnlyList<string> InvestigationLinks);

public sealed record AccessLedgerEntryDto(string Time, string Message, string Tone);

public sealed record AiOversightPanelDto(
    string CoreLabel,
    string StatusBadge,
    decimal ConfidencePercent,
    string InferenceMs,
    string ModelVersion);

public sealed record TeamDirectoryDto(
    IReadOnlyList<TeamMemberDto> Members,
    AiOversightPanelDto AiOversight,
    IReadOnlyList<AccessLedgerEntryDto> Ledger);

public sealed record TerminalLogLineDto(
    string Timestamp,
    string Level,
    string Category,
    string Message,
    string RowVariant);
