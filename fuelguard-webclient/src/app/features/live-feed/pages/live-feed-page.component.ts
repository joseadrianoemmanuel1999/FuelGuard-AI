import { ChangeDetectionStrategy, Component, computed, inject, OnInit } from '@angular/core';
import type { LiveFeedEventDto } from '../../../core/models/portal.models';
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
  protected readonly dashboardState = inject(DashboardStateService);

  protected readonly liveFeedEvents = computed<LiveFeedEventDto[]>(() => this.dashboardState.liveFeed());

  protected readonly formattedAiConfidencePercent = computed<string>(() => {
    const riskScore = this.dashboardState.risk().score;
    const confidencePercentValue = riskScore < 50 ? 89.4 : 100 - riskScore / 10;
    return confidencePercentValue.toFixed(1);
  });

  ngOnInit(): void {
    void this.dashboardState.refresh();
  }
}
