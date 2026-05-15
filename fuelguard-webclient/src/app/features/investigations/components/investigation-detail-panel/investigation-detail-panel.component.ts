import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import type { Investigation, TimelineEvent } from '../../models/investigation.models';

const BASELINE_FLOW_STRIP_BAR_COUNT = 3;

@Component({
  selector: 'app-investigation-detail-panel',
  standalone: true,
  templateUrl: './investigation-detail-panel.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InvestigationDetailPanelComponent {
  private readonly domSanitizer = inject(DomSanitizer);

  readonly selectedInvestigation = input.required<Investigation>();

  protected readonly sanitizedNarrativeHtml = computed<SafeHtml>(() =>
    this.domSanitizer.bypassSecurityTrustHtml(this.selectedInvestigation().detail.narrativeHtml),
  );

  protected cssClassForTimelineDotTone(tone: TimelineEvent['dotTone']): string {
    switch (tone) {
      case 'error':
        return 'bg-error';
      case 'primary':
        return 'bg-primary';
      case 'neutral':
        return 'bg-outline-variant';
      default: {
        const exhaustiveCheck: never = tone;
        return exhaustiveCheck;
      }
    }
  }

  protected onDeploySurveillanceDrone(): void {
    console.info('[FuelGuard] Deploy Surveillance Drone (mock)', this.selectedInvestigation().id);
  }

  protected onEmergencyValveShut(): void {
    console.info(
      '[FuelGuard] Emergency shut valve (mock)',
      this.selectedInvestigation().detail.telemetryNodeLabel,
    );
  }

  protected onEscalateToFederalOversight(): void {
    console.info('[FuelGuard] Escalate to Federal Oversight (mock)', this.selectedInvestigation().id);
  }

  protected cssClassForFlowStripBar(barIndex: number): string {
    const baseShell = 'flex-1 rounded-t border-t transition-all';
    if (barIndex < BASELINE_FLOW_STRIP_BAR_COUNT) {
      return `${baseShell} bg-tertiary/20 border-transparent`;
    }
    return `${baseShell} bg-error/40 border-error animate-pulse`;
  }
}
