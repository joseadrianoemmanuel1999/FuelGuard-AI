import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { DashboardStateService } from '../../../../core/services/dashboard-state.service';

@Component({
  selector: 'app-hero-section',
  standalone: true,
  templateUrl: './hero-section.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeroSectionComponent {
  private readonly dashboard = inject(DashboardStateService);

  protected runSimulation(): void {
    void this.dashboard.simulateFuelSmugglingScenario();
  }
}
