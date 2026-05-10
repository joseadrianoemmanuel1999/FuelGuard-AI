import {
  ChangeDetectionStrategy,
  Component,
  computed,
  input,
} from '@angular/core';
import { riskCardPulse } from '../../dashboard.animations';

@Component({
  selector: 'app-risk-overview-card',
  standalone: true,
  templateUrl: './risk-overview-card.component.html',
  animations: [riskCardPulse],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RiskOverviewCardComponent {
  readonly score = input(87);
  readonly levelLabel = input('Risk Level: HIGH');

  readonly circumference = 2 * Math.PI * 88;
  readonly dashOffset = computed(() => {
    const pct = Math.min(100, Math.max(0, this.score())) / 100;
    return this.circumference * (1 - pct);
  });

  readonly pulseKey = computed(() => String(this.score()));
}
