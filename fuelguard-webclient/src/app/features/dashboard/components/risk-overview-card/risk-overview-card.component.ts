import {
  ChangeDetectionStrategy,
  Component,
  computed,
  input,
} from '@angular/core';

@Component({
  selector: 'app-risk-overview-card',
  standalone: true,
  templateUrl: './risk-overview-card.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RiskOverviewCardComponent {
  readonly score = input<number>(87);
  readonly levelLabel = input<string>('Risk Level: HIGH');

  readonly circumference: number = 2 * Math.PI * 88;
  readonly dashOffset = computed<number>(() => {
    const pct: number = Math.min(100, Math.max(0, this.score())) / 100;
    return this.circumference * (1 - pct);
  });
}
