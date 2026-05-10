import { ChangeDetectionStrategy, Component, computed, inject, OnInit } from '@angular/core';
import { DashboardStateService } from '../../../core/services/dashboard-state.service';
import { LiveFeedStreamComponent } from '../components/live-feed-stream/live-feed-stream.component';

@Component({
  selector: 'app-live-feed-page',
  standalone: true,
  imports: [LiveFeedStreamComponent],
  templateUrl: './live-feed-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LiveFeedPageComponent implements OnInit {
  protected readonly state = inject(DashboardStateService);
  protected readonly events = computed(() => this.state.liveFeed());

  /** Single formatted value for strict template typing (avoids number | string in ternary). */
  protected readonly aiConfidenceIndex = computed(() => {
    const s = this.state.risk().score;
    const v = s < 50 ? 89.4 : 100 - s / 10;
    return v.toFixed(1);
  });

  ngOnInit(): void {
    void this.state.refresh();
  }
}
