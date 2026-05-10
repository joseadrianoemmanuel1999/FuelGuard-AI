export interface LiveFeedEventDto {
  timestamp: string;
  category: string;
  title: string;
  body: string;
  tags: string[];
  accent: string;
}

export interface InvestigationBoardRowDto {
  companyName: string;
  investigationId: string;
  riskLabel: string;
  riskTone: string;
  confidencePercent: number;
  region: string;
  status: string;
  statusActiveDot: boolean;
  lastActivity: string;
}

export interface InvestigationTimelineItemDto {
  timeUtc: string;
  title: string;
  dotTone: string;
}

export interface InvestigationDetailDto {
  caseId: string;
  narrativeHtml: string;
  timeline: InvestigationTimelineItemDto[];
  normalFlowLitersPerSecond: number;
  actualFlowLitersPerSecond: number;
}

export interface InvestigationsBoardDto {
  rows: InvestigationBoardRowDto[];
  selectedDetail: InvestigationDetailDto;
}

export interface ExecutiveSummaryCardDto {
  label: string;
  value: string;
  subtext: string;
  borderTone: string;
  iconName: string;
}

export interface PipelineSectorDto {
  code: string;
  statusLabel: string;
  statusTone: string;
  description: string;
  riskIndex: number;
  footerLabel: string;
  footerTone: string;
}

export interface ExecutiveRiskReportDto {
  summaryCards: ExecutiveSummaryCardDto[];
  sectors: PipelineSectorDto[];
  mapImageUrl: string;
  chartAnnotation: string;
}

export interface TeamMemberDto {
  name: string;
  role: string;
  level: number;
  status: string;
  assignment: string;
  avatarUrl: string;
  onlineDot: boolean;
  investigationLinks: string[];
}

export interface AccessLedgerEntryDto {
  time: string;
  message: string;
  tone: string;
}

export interface AiOversightPanelDto {
  coreLabel: string;
  statusBadge: string;
  confidencePercent: number;
  inferenceMs: string;
  modelVersion: string;
}

export interface TeamDirectoryDto {
  members: TeamMemberDto[];
  aiOversight: AiOversightPanelDto;
  ledger: AccessLedgerEntryDto[];
}

export interface TerminalLogLineDto {
  timestamp: string;
  level: string;
  category: string;
  message: string;
  rowVariant: string;
}
