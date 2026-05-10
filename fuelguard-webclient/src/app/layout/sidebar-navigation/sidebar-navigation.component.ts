import { ChangeDetectionStrategy, Component, inject, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { DashboardStateService } from '../../core/services/dashboard-state.service';

@Component({
  selector: 'app-sidebar-navigation',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar-navigation.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidebarNavigationComponent {
  private readonly dashboard = inject(DashboardStateService);

  readonly mobileOpen = input(false);
  readonly closeMobile = output<void>();

  protected runSimulation(): void {
    void this.dashboard.simulateFuelSmugglingScenario();
  }
}
