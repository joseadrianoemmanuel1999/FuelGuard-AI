import { inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { FuelGuardApiService } from '../../../core/services/fuel-guard-api.service';
import type {
  InvestigationBoardRowDto,
  InvestigationDetailDto,
  InvestigationsBoardDto,
} from '../../../core/models/portal.models';
import type { Investigation, InvestigationDetail, TimelineEvent } from '../models/investigation.models';

function mapRiskTier(tone: string): Investigation['riskTier'] {
  if (tone === 'critical') return 'critical';
  if (tone === 'high') return 'high';
  if (tone === 'low') return 'low';
  return 'medium';
}

function mapTimeline(dto: InvestigationDetailDto): TimelineEvent[] {
  return dto.timeline.map((t) => {
    let dot: TimelineEvent['dotTone'] = 'neutral';
    if (t.dotTone === 'error') dot = 'error';
    else if (t.dotTone === 'primary') dot = 'primary';
    return { timeUtc: t.timeUtc, title: t.title, dotTone: dot };
  });
}

function detailFromDto(d: InvestigationDetailDto): InvestigationDetail {
  return {
    caseId: d.caseId,
    narrativeHtml: d.narrativeHtml,
    timeline: mapTimeline(d),
    normalFlowLitersPerSecond: d.normalFlowLitersPerSecond,
    actualFlowLitersPerSecond: d.actualFlowLitersPerSecond,
    telemetryNodeLabel: 'Node 42',
    telemetryBarsPercent: [80, 75, 80, 50, 45, 50],
  };
}

function syntheticDetail(row: InvestigationBoardRowDto): InvestigationDetail {
  const node = 40 + (row.confidencePercent % 12);
  return {
    caseId: row.investigationId,
    narrativeHtml: `<p class="text-xs leading-relaxed text-on-surface/90">Operational summary for <span class="text-primary font-data-tabular">${row.companyName}</span>. Correlation engine is assembling timeline artifacts for <span class="text-primary font-data-tabular">Node ${node}</span>.</p>`,
    timeline: [],
    normalFlowLitersPerSecond: 420,
    actualFlowLitersPerSecond: Math.max(
      200,
      Math.round(420 - row.confidencePercent * 0.45),
    ),
    telemetryNodeLabel: `Node ${node}`,
    telemetryBarsPercent: [70, 72, 68, 55, 52, 58],
  };
}

function satelliteBlurb(row: InvestigationBoardRowDto): string {
  if (row.investigationId === 'INV-4029-A') {
    return 'Live spectral analysis of PetroFlow Tanker 0928 detected in unauthorized zone.';
  }
  return `Live spectral analysis — ${row.companyName} corridor; multi-spectral feed correlating AIS and berth telemetry.`;
}

function investigationFromRow(
  row: InvestigationBoardRowDto,
  rowIndex: number,
  board: { selectedDetail: InvestigationDetailDto },
): Investigation {
  const detail =
    board.selectedDetail.caseId === row.investigationId
      ? detailFromDto(board.selectedDetail)
      : syntheticDetail(row);
  return {
    id: row.investigationId,
    company: row.companyName,
    companySubtitle: rowIndex === 0 ? `ID: ${row.investigationId}` : null,
    riskLevel: row.riskLabel,
    riskTier: mapRiskTier(row.riskTone),
    confidence: row.confidencePercent,
    region: row.region,
    status: row.status,
    statusActiveDot: row.statusActiveDot,
    lastActivity: row.lastActivity,
    detail,
    satelliteNarrative: satelliteBlurb(row),
  };
}

@Injectable({ providedIn: 'root' })
export class InvestigationStateService {
  private readonly api = inject(FuelGuardApiService);

  private readonly board = signal<InvestigationsBoardDto | null>(null);

  readonly investigations = signal<Investigation[]>([]);
  readonly selectedInvestigation = signal<Investigation | null>(null);
  readonly loading = signal(false);

  /** Histogram column heights (percent of track), matching reference layout. */
  readonly intelligenceBarPercents = signal<readonly number[]>([40, 55, 70, 95, 30, 45, 85, 100]);

  async loadInvestigations(): Promise<void> {
    this.loading.set(true);
    try {
      const dto = await firstValueFrom(this.api.getInvestigations());
      this.board.set(dto);
      this.investigations.set(dto.rows.map((r, i) => investigationFromRow(r, i, dto)));
      this.selectInvestigation(dto.selectedDetail.caseId);
    } catch (err) {
      console.error('[Investigations] load failed', err);
      this.board.set(null);
      this.investigations.set([]);
      this.selectedInvestigation.set(null);
    } finally {
      this.loading.set(false);
    }
  }

  selectInvestigation(id: string): void {
    const dto = this.board();
    if (!dto) return;
    const row = dto.rows.find((r) => r.investigationId === id);
    if (!row) return;
    const index = dto.rows.indexOf(row);
    const detail =
      dto.selectedDetail.caseId === id ? detailFromDto(dto.selectedDetail) : syntheticDetail(row);
    this.selectedInvestigation.set({
      id: row.investigationId,
      company: row.companyName,
      companySubtitle: index === 0 ? `ID: ${row.investigationId}` : null,
      riskLevel: row.riskLabel,
      riskTier: mapRiskTier(row.riskTone),
      confidence: row.confidencePercent,
      region: row.region,
      status: row.status,
      statusActiveDot: row.statusActiveDot,
      lastActivity: row.lastActivity,
      detail,
      satelliteNarrative: satelliteBlurb(row),
    });
  }
}
