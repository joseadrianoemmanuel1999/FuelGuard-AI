import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DashboardStateService } from '../core/services/dashboard-state.service';
import { SidebarNavigationComponent } from './sidebar-navigation/sidebar-navigation.component';
import { ShellTopNavbarComponent } from './top-navbar/shell-top-navbar.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, SidebarNavigationComponent, ShellTopNavbarComponent],
  templateUrl: './app-layout.component.html',
  host: { class: 'block min-h-screen' },
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppLayoutComponent {
  protected readonly state = inject(DashboardStateService);

  protected toggleNav(): void {
    this.state.setMobileSidebar(!this.state.mobileSidebarOpen());
  }
}
