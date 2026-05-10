import { ChangeDetectionStrategy, Component, computed, HostBinding, input, output } from '@angular/core';
import type { Investigation } from '../../models/investigation.models';

@Component({
  selector: 'tr[appInvestigationRow]',
  standalone: true,
  templateUrl: './investigation-row.component.html',
  host: {
    '(click)': 'onRowClick()',
  },
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InvestigationRowComponent {
  readonly investigation = input.required<Investigation>();
  readonly selected = input(false);
  readonly select = output<string>();

  protected readonly rowClass = computed(() => {
    const base = 'transition-colors cursor-pointer border-l-2';
    return this.selected()
      ? `${base} bg-primary/5 hover:bg-primary/10 border-primary`
      : `${base} hover:bg-surface-container-highest/50 border-transparent`;
  });

  @HostBinding('class')
  protected get hostRowClass(): string {
    return this.rowClass();
  }

  protected readonly riskBadgeClass = computed(() => {
    const base = 'px-2 py-0.5 rounded-full text-[11px] font-bold font-data-tabular';
    switch (this.investigation().riskTier) {
      case 'critical':
        return `${base} bg-error-container/20 text-error border border-error/30 risk-glow-high`;
      case 'high':
        return `${base} bg-error-container/20 text-error border border-error/30`;
      case 'low':
        return `${base} bg-tertiary-container/20 text-tertiary border border-tertiary/30`;
      default:
        return `${base} bg-secondary-container/30 text-on-surface border border-outline-variant risk-glow-med`;
    }
  });

  protected readonly confidenceFillClass = computed(() => {
    const t = this.investigation().riskTier;
    if (t === 'low') return 'bg-tertiary-container';
    if (t === 'medium') return 'bg-secondary-container';
    return 'bg-primary';
  });

  protected readonly confidenceLabelClass = computed(() => {
    const t = this.investigation().riskTier;
    if (t === 'low') return 'text-[11px] text-tertiary mt-1 block';
    if (t === 'medium') return 'text-[11px] text-on-surface-variant mt-1 block';
    return 'text-[11px] text-primary mt-1 block';
  });

  protected onRowClick(): void {
    this.select.emit(this.investigation().id);
  }
}
