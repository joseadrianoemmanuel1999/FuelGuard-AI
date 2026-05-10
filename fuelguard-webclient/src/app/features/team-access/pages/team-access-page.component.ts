import { ChangeDetectionStrategy, Component, computed, inject, OnInit } from '@angular/core';
import { DashboardStateService } from '../../../core/services/dashboard-state.service';
import { AiOversightPanelComponent } from '../components/ai-oversight-panel/ai-oversight-panel.component';
import { UserCardComponent } from '../components/user-card/user-card.component';

@Component({
  selector: 'app-team-access-page',
  standalone: true,
  imports: [UserCardComponent, AiOversightPanelComponent],
  templateUrl: './team-access-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TeamAccessPageComponent implements OnInit {
  protected readonly state = inject(DashboardStateService);
  protected readonly directory = computed(() => this.state.team());

  ngOnInit(): void {
    void this.state.refresh();
  }
}
