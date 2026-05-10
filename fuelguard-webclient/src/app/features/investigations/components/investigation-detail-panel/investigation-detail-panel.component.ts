import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import type { Investigation } from '../../models/investigation.models';

@Component({
  selector: 'app-investigation-detail-panel',
  standalone: true,
  templateUrl: './investigation-detail-panel.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InvestigationDetailPanelComponent {
  private readonly sanitizer = inject(DomSanitizer);

  readonly investigation = input.required<Investigation>();

  protected readonly safeNarrative = computed(() =>
    this.sanitizer.bypassSecurityTrustHtml(this.investigation().detail.narrativeHtml),
  );

  protected dotClass(tone: string): string {
    if (tone === 'error') return 'bg-error';
    if (tone === 'primary') return 'bg-primary';
    return 'bg-outline-variant';
  }

  protected deployDrone(): void {
    console.info('[FuelGuard] Deploy Surveillance Drone (mock)', this.investigation().id);
  }

  protected shutValve(): void {
    console.info('[FuelGuard] Emergency shut valve (mock)', this.investigation().detail.telemetryNodeLabel);
  }

  protected escalateCase(): void {
    console.info('[FuelGuard] Escalate to Federal Oversight (mock)', this.investigation().id);
  }

  protected telemetryBarClass(i: number): string {
    const base = 'flex-1 rounded-t border-t transition-all';
    if (i < 3) return `${base} bg-tertiary/20 border-transparent`;
    return `${base} bg-error/40 border-error animate-pulse`;
  }
}
