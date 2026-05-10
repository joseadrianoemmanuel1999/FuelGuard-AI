import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import type { LiveFeedEventDto } from '../../../../core/models/portal.models';
import { EventCardComponent } from '../event-card/event-card.component';

@Component({
  selector: 'app-live-feed-stream',
  standalone: true,
  imports: [EventCardComponent],
  templateUrl: './live-feed-stream.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LiveFeedStreamComponent {
  readonly events = input<LiveFeedEventDto[]>([]);
}
