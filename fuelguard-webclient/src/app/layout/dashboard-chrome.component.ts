import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-dashboard-chrome',
  standalone: true,
  template:
    '<div class="max-w-container-max mx-auto space-y-stack-lg"><ng-content /></div>',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardChromeComponent {}
