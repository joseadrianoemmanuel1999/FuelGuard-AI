export interface NarrativeTokenDto {
  text: string;
  tone: 'default' | 'primary' | 'error';
}

export interface RecommendedActionDto {
  title: string;
  materialIcon: string;
  accent: 'error' | 'tertiary' | 'primary';
}

export interface AiInvestigationDto {
  summaryTokens: NarrativeTokenDto[];
  recommendedActions: RecommendedActionDto[];
}

export interface RiskAssessmentDto {
  score: number;
  level: string;
  levelLabel: string;
  summary: string;
}

export interface PipelineLogDto {
  timestamp: string;
  stage: string;
  message: string;
}

export interface AlertDto {
  id: string;
  title: string;
  severity: string;
  detectedAt: string;
  detail: string;
}

export interface TelemetryDto {
  flowRateLitersPerSecond: number;
  pressureBar: number;
  temperatureCelsius: number;
  statusLabel: string;
  panelTitle: string;
  mapBadgeText: string;
}

export interface CommandCenterSimulationResultDto {
  risk: RiskAssessmentDto;
  investigation: AiInvestigationDto;
  pipelineLogs: PipelineLogDto[];
  alerts: AlertDto[];
  telemetry: TelemetryDto;
  activePipelineStageIndex: number;
  alertStateActive: boolean;
}
