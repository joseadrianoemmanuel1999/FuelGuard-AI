/** Timeline entry in the investigation detail panel (Anomaly Timeline). */
export interface TimelineEvent {
  timeUtc: string;
  title: string;
  dotTone: 'error' | 'primary' | 'neutral';
}

/** Detail payload for the right-hand case panel. */
export interface InvestigationDetail {
  caseId: string;
  narrativeHtml: string;
  timeline: TimelineEvent[];
  normalFlowLitersPerSecond: number;
  actualFlowLitersPerSecond: number;
  /** Shown in telemetry header, e.g. "Node 42". */
  telemetryNodeLabel: string;
  /** Six bar heights (0–100) for the mini telemetry strip, left-to-right. */
  telemetryBarsPercent: readonly number[];
}

/** Row + resolved detail for the investigations experience. */
export interface Investigation {
  id: string;
  company: string;
  /** When set, renders the stacked ID line under the company (matches reference row 1). */
  companySubtitle: string | null;
  riskLevel: string;
  riskTier: 'critical' | 'high' | 'medium' | 'low';
  confidence: number;
  region: string;
  status: string;
  statusActiveDot: boolean;
  lastActivity: string;
  detail: InvestigationDetail;
  /** Satellite overlay body copy. */
  satelliteNarrative: string;
}
