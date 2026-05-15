import { inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { FuelGuardApiService } from '../../../core/services/fuel-guard-api.service';
import type {
  InvestigationBoardRowDto,
  InvestigationDetailDto,
  InvestigationsBoardDto,
} from '../../../core/models/portal.models';
import type { Investigation, InvestigationDetail, TimelineEvent } from '../models/investigation.models';

function mapRiskTier(riskToneRaw: string): Investigation['riskTier'] {
  if (riskToneRaw === 'critical') return 'critical';
  if (riskToneRaw === 'high') return 'high';
  if (riskToneRaw === 'low') return 'low';
  return 'medium';
}

function mapTimelineEventsFromDetail(detailDto: InvestigationDetailDto): TimelineEvent[] {
  return detailDto.timeline.map((timelineEntryDto) => {
    let resolvedDotTone: TimelineEvent['dotTone'] = 'neutral';
    if (timelineEntryDto.dotTone === 'error') resolvedDotTone = 'error';
    else if (timelineEntryDto.dotTone === 'primary') resolvedDotTone = 'primary';
    return {
      timeUtc: timelineEntryDto.timeUtc,
      title: timelineEntryDto.title,
      dotTone: resolvedDotTone,
    };
  });
}

function investigationDetailFromApiDto(detailDto: InvestigationDetailDto): InvestigationDetail {
  return {
    caseId: detailDto.caseId,
    narrativeHtml: detailDto.narrativeHtml,
    timeline: mapTimelineEventsFromDetail(detailDto),
    normalFlowLitersPerSecond: detailDto.normalFlowLitersPerSecond,
    actualFlowLitersPerSecond: detailDto.actualFlowLitersPerSecond,
    telemetryNodeLabel: 'Node 42',
    telemetryBarsPercent: [80, 75, 80, 50, 45, 50] as const,
  };
}

function syntheticInvestigationDetail(boardRow: InvestigationBoardRowDto): InvestigationDetail {
  const syntheticTelemetryNodeNumber = 40 + (boardRow.confidencePercent % 12);
  return {
    caseId: boardRow.investigationId,
    narrativeHtml: `<p class="text-xs leading-relaxed text-on-surface/90">Operational summary for <span class="text-primary font-data-tabular">${boardRow.companyName}</span>. Correlation engine is assembling timeline artifacts for <span class="text-primary font-data-tabular">Node ${syntheticTelemetryNodeNumber}</span>.</p>`,
    timeline: [],
    normalFlowLitersPerSecond: 420,
    actualFlowLitersPerSecond: Math.max(
      200,
      Math.round(420 - boardRow.confidencePercent * 0.45),
    ),
    telemetryNodeLabel: `Node ${syntheticTelemetryNodeNumber}`,
    telemetryBarsPercent: [70, 72, 68, 55, 52, 58] as const,
  };
}

function buildSatelliteNarrativeForRow(boardRow: InvestigationBoardRowDto): string {
  if (boardRow.investigationId === 'INV-4029-A') {
    return 'Live spectral analysis of PetroFlow Tanker 0928 detected in unauthorized zone.';
  }
  return `Live spectral analysis — ${boardRow.companyName} corridor; multi-spectral feed correlating AIS and berth telemetry.`;
}

function investigationFromBoardRow(
  boardRow: InvestigationBoardRowDto,
  boardRowIndex: number,
  investigationsBoard: InvestigationsBoardDto,
): Investigation {
  const investigationDetail =
    investigationsBoard.selectedDetail.caseId === boardRow.investigationId
      ? investigationDetailFromApiDto(investigationsBoard.selectedDetail)
      : syntheticInvestigationDetail(boardRow);
  return {
    id: boardRow.investigationId,
    company: boardRow.companyName,
    companySubtitle: boardRowIndex === 0 ? `ID: ${boardRow.investigationId}` : null,
    riskLevel: boardRow.riskLabel,
    riskTier: mapRiskTier(boardRow.riskTone),
    confidence: boardRow.confidencePercent,
    region: boardRow.region,
    status: boardRow.status,
    statusActiveDot: boardRow.statusActiveDot,
    lastActivity: boardRow.lastActivity,
    detail: investigationDetail,
    satelliteNarrative: buildSatelliteNarrativeForRow(boardRow),
  };
}

@Injectable({ providedIn: 'root' })
export class InvestigationStateService {
  private readonly fuelGuardApi = inject(FuelGuardApiService);

  private readonly investigationsBoard = signal<InvestigationsBoardDto | null>(null);

  readonly investigations = signal<Investigation[]>([]);
  readonly selectedInvestigation = signal<Investigation | null>(null);
  readonly loading = signal(false);

  readonly histogramBarHeightPercents = signal<readonly number[]>([
    40, 55, 70, 95, 30, 45, 85, 100,
  ] as const);

  async loadInvestigations(): Promise<void> {
    this.loading.set(true);
    try {
      const investigationsBoardDto = await firstValueFrom(this.fuelGuardApi.getInvestigations());
      this.investigationsBoard.set(investigationsBoardDto);
      this.investigations.set(
        investigationsBoardDto.rows.map((boardRow, boardRowIndex) =>
          investigationFromBoardRow(boardRow, boardRowIndex, investigationsBoardDto),
        ),
      );
      this.selectInvestigation(investigationsBoardDto.selectedDetail.caseId);
    } catch (error) {
      console.error('[Investigations] load failed', error);
      this.investigationsBoard.set(null);
      this.investigations.set([]);
      this.selectedInvestigation.set(null);
    } finally {
      this.loading.set(false);
    }
  }

  selectInvestigation(investigationId: string): void {
    const investigationsBoardDto = this.investigationsBoard();
    if (!investigationsBoardDto) return;
    const matchingBoardRow = investigationsBoardDto.rows.find(
      (boardRow) => boardRow.investigationId === investigationId,
    );
    if (!matchingBoardRow) return;
    const boardRowIndex = investigationsBoardDto.rows.indexOf(matchingBoardRow);
    const resolvedInvestigationDetail =
      investigationsBoardDto.selectedDetail.caseId === investigationId
        ? investigationDetailFromApiDto(investigationsBoardDto.selectedDetail)
        : syntheticInvestigationDetail(matchingBoardRow);
    this.selectedInvestigation.set({
      id: matchingBoardRow.investigationId,
      company: matchingBoardRow.companyName,
      companySubtitle: boardRowIndex === 0 ? `ID: ${matchingBoardRow.investigationId}` : null,
      riskLevel: matchingBoardRow.riskLabel,
      riskTier: mapRiskTier(matchingBoardRow.riskTone),
      confidence: matchingBoardRow.confidencePercent,
      region: matchingBoardRow.region,
      status: matchingBoardRow.status,
      statusActiveDot: matchingBoardRow.statusActiveDot,
      lastActivity: matchingBoardRow.lastActivity,
      detail: resolvedInvestigationDetail,
      satelliteNarrative: buildSatelliteNarrativeForRow(matchingBoardRow),
    });
  }
}
