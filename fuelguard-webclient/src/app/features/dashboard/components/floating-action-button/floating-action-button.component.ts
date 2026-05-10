import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { DashboardStateService } from '../../../../core/services/dashboard-state.service';

@Component({
  selector: 'app-floating-action-button',
  standalone: true,
  templateUrl: './floating-action-button.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FloatingActionButtonComponent {
  private readonly dashboard = inject(DashboardStateService);

  protected runSimulation(): void {
    void this.dashboard.simulateFuelSmugglingScenario();
  }
}
