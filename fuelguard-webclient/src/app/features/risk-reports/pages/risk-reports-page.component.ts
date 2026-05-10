import { ChangeDetectionStrategy, Component, computed, inject, OnInit } from '@angular/core';
import { DashboardStateService } from '../../../core/services/dashboard-state.service';
import { AnalyticsCardsComponent } from '../components/analytics-cards/analytics-cards.component';
import { AnomalyChartComponent } from '../components/anomaly-chart/anomaly-chart.component';
import { RiskMapComponent } from '../components/risk-map/risk-map.component';

@Component({
  selector: 'app-risk-reports-page',
  standalone: true,
  imports: [AnalyticsCardsComponent, AnomalyChartComponent, RiskMapComponent],
  templateUrl: './risk-reports-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RiskReportsPageComponent implements OnInit {
  protected readonly state = inject(DashboardStateService);
  protected readonly report = computed(() => this.state.reports());

  ngOnInit(): void {
    void this.state.refresh();
  }
}
