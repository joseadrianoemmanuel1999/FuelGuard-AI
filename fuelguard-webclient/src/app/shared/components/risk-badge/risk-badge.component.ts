import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

export type RiskBadgeTone = 'critical' | 'high' | 'medium' | 'low';

@Component({
  selector: 'app-risk-badge',
  standalone: true,
  templateUrl: './risk-badge.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RiskBadgeComponent {
  readonly label = input.required<string>();
  readonly tone = input<RiskBadgeTone>('medium');

  protected readonly klass = computed(() => {
    const t = this.tone();
    const base =
      'px-2 py-0.5 rounded-full text-[11px] font-bold border font-data-tabular whitespace-nowrap';
    switch (t) {
      case 'critical':
        return `${base} bg-error-container/20 text-error border-error/30 risk-glow-high`;
      case 'high':
        return `${base} bg-error-container/20 text-error border-error/30 risk-glow-high`;
      case 'low':
        return `${base} bg-tertiary-container/20 text-tertiary border-tertiary/30`;
      default:
        return `${base} bg-secondary-container/30 text-on-surface border-outline-variant risk-glow-med`;
    }
  });
}
