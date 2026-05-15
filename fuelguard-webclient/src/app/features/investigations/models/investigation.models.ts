export interface TimelineEvent {
  timeUtc: string;
  title: string;
  dotTone: 'error' | 'primary' | 'neutral';
}

export interface InvestigationDetail {
  caseId: string;
  narrativeHtml: string;
  timeline: TimelineEvent[];
  normalFlowLitersPerSecond: number;
  actualFlowLitersPerSecond: number;
  telemetryNodeLabel: string;
  telemetryBarsPercent: readonly number[];
}

export interface Investigation {
  id: string;
  company: string;
  companySubtitle: string | null;
  riskLevel: string;
  riskTier: 'critical' | 'high' | 'medium' | 'low';
  confidence: number;
  region: string;
  status: string;
  statusActiveDot: boolean;
  lastActivity: string;
  detail: InvestigationDetail;
  satelliteNarrative: string;
}
