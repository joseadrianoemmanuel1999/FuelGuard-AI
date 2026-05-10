import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { InvestigationDetailPanelComponent } from '../components/investigation-detail-panel/investigation-detail-panel.component';
import { InvestigationsTableComponent } from '../components/investigations-table/investigations-table.component';
import { IntelligenceChartComponent } from '../components/intelligence-chart/intelligence-chart.component';
import { SatelliteCardComponent } from '../components/satellite-card/satellite-card.component';
import { InvestigationStateService } from '../services/investigation-state.service';

@Component({
  selector: 'app-investigations-page',
  standalone: true,
  imports: [
    InvestigationsTableComponent,
    InvestigationDetailPanelComponent,
    IntelligenceChartComponent,
    SatelliteCardComponent,
  ],
  templateUrl: './investigations.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InvestigationsPage implements OnInit {
  protected readonly state = inject(InvestigationStateService);

  ngOnInit(): void {
    void this.state.loadInvestigations();
  }

  protected onSelect(id: string): void {
    this.state.selectInvestigation(id);
  }
}
