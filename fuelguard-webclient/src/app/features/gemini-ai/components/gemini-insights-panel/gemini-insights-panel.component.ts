import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { CardContainerComponent } from '../../../../shared/components/card-container/card-container.component';
import type { GeminiInsightsResponseDto } from '../../models/gemini.models';

@Component({
  selector: 'app-gemini-insights-panel',
  standalone: true,
  imports: [CardContainerComponent],
  templateUrl: './gemini-insights-panel.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GeminiInsightsPanelComponent {
  readonly insights = input<GeminiInsightsResponseDto | null>(null);
  readonly loading = input(false);
  readonly error = input<string | null>(null);

  protected riskClass(level: string): string {
    const l = level.toUpperCase();
    if (l === 'CRITICAL' || l === 'HIGH') return 'text-error';
    if (l === 'MEDIUM') return 'text-tertiary';
    return 'text-primary';
  }
}
