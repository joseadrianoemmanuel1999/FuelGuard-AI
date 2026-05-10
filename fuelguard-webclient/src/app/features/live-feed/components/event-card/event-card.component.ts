import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import type { LiveFeedEventDto } from '../../../../core/models/portal.models';

@Component({
  selector: 'app-event-card',
  standalone: true,
  templateUrl: './event-card.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EventCardComponent {
  readonly event = input.required<LiveFeedEventDto>();

  protected readonly borderClass = computed(() => {
    const a = this.event().accent;
    if (a === 'error') return 'border-error/40 bg-error-container/10';
    if (a === 'primary') return 'border-primary/40 bg-primary/5';
    if (a === 'warning') return 'border-[#FFAB40]/40 bg-[#FFAB40]/5';
    return 'border-outline-variant/30 bg-surface-container-low/50';
  });

  protected readonly tagClass = computed(() => {
    const a = this.event().accent;
    if (a === 'error') return 'bg-error/20 text-error';
    if (a === 'primary') return 'bg-primary/20 text-primary';
    if (a === 'warning') return 'bg-[#FFAB40]/20 text-[#FFAB40]';
    return 'bg-surface-container-highest text-on-surface-variant';
  });
}
