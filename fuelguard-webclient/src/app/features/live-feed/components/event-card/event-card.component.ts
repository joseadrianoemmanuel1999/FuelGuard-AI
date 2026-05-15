import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import type { LiveFeedEventDto } from '../../../../core/models/portal.models';

type ResolvedEventAccent = 'error' | 'primary' | 'warning' | 'default';

const EVENT_CARD_BORDER_BY_ACCENT: Readonly<Record<ResolvedEventAccent, string>> = {
  error: 'border-error/40 bg-error-container/10',
  primary: 'border-primary/40 bg-primary/5',
  warning: 'border-[#FFAB40]/40 bg-[#FFAB40]/5',
  default: 'border-outline-variant/30 bg-surface-container-low/50',
};

const CATEGORY_BADGE_TONE_BY_ACCENT: Readonly<Record<ResolvedEventAccent, string>> = {
  error: 'bg-error/20 text-error',
  primary: 'bg-primary/20 text-primary',
  warning: 'bg-[#FFAB40]/20 text-[#FFAB40]',
  default: 'bg-surface-container-highest text-on-surface-variant',
};

const CATEGORY_BADGE_LAYOUT = 'px-2 py-0.5 rounded text-[10px] font-bold tracking-wider';

function resolveEventAccent(rawAccent: LiveFeedEventDto['accent']): ResolvedEventAccent {
  switch (rawAccent) {
    case 'error':
      return 'error';
    case 'primary':
      return 'primary';
    case 'warning':
      return 'warning';
    default:
      return 'default';
  }
}

@Component({
  selector: 'app-event-card',
  standalone: true,
  templateUrl: './event-card.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EventCardComponent {
  readonly liveFeedEvent = input.required<LiveFeedEventDto>();

  protected readonly eventCardBorderClass = computed<string>(() => {
    const accent = resolveEventAccent(this.liveFeedEvent().accent);
    return EVENT_CARD_BORDER_BY_ACCENT[accent];
  });

  protected readonly categoryBadgeClass = computed<string>(() => {
    const accent = resolveEventAccent(this.liveFeedEvent().accent);
    return `${CATEGORY_BADGE_TONE_BY_ACCENT[accent]} ${CATEGORY_BADGE_LAYOUT}`;
  });
}
