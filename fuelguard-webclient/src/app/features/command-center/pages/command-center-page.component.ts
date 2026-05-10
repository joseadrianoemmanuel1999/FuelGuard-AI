import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { AiInvestigationPanelComponent } from '../../dashboard/components/ai-investigation-panel/ai-investigation-panel.component';
import { FloatingActionButtonComponent } from '../../dashboard/components/floating-action-button/floating-action-button.component';
import { HeroSectionComponent } from '../../dashboard/components/hero-section/hero-section.component';
import { PipelineVisualizationComponent } from '../../dashboard/components/pipeline-visualization/pipeline-visualization.component';
import { RiskOverviewCardComponent } from '../../dashboard/components/risk-overview-card/risk-overview-card.component';
import { SystemLogsTerminalComponent } from '../../dashboard/components/system-logs-terminal/system-logs-terminal.component';
import { TelemetryMapPanelComponent } from '../../dashboard/components/telemetry-map-panel/telemetry-map-panel.component';
import { DashboardChromeComponent } from '../../../layout/dashboard-chrome.component';
import { DashboardStateService } from '../../../core/services/dashboard-state.service';

@Component({
  selector: 'app-command-center-page',
  standalone: true,
  imports: [
    HeroSectionComponent,
    RiskOverviewCardComponent,
    PipelineVisualizationComponent,
    AiInvestigationPanelComponent,
    TelemetryMapPanelComponent,
    SystemLogsTerminalComponent,
    FloatingActionButtonComponent,
    DashboardChromeComponent,
  ],
  templateUrl: './command-center-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommandCenterPageComponent implements OnInit {
  protected readonly state = inject(DashboardStateService);

  ngOnInit(): void {
    void this.state.refresh();
  }
}
