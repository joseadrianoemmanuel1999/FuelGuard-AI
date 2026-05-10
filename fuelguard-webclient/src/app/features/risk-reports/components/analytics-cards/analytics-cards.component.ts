import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import type { ExecutiveSummaryCardDto } from '../../../../core/models/portal.models';

@Component({
  selector: 'app-analytics-cards',
  standalone: true,
  templateUrl: './analytics-cards.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AnalyticsCardsComponent {
  readonly cards = input<ExecutiveSummaryCardDto[]>([]);

  protected borderClass(tone: string): string {
    if (tone === 'error') return 'border-l-error';
    if (tone === 'tertiary') return 'border-l-tertiary';
    if (tone === 'primary-container') return 'border-l-primary-container';
    return 'border-l-primary';
  }

  protected iconClass(tone: string): string {
    if (tone === 'error') return 'text-error';
    if (tone === 'tertiary') return 'text-tertiary';
    if (tone === 'primary-container') return 'text-primary-container';
    return 'text-primary';
  }

  protected subtextClass(tone: string): string {
    if (tone === 'error') return 'text-error';
    return 'text-tertiary';
  }
}
