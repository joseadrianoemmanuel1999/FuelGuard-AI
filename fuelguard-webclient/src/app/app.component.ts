import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: '<router-outlet></router-outlet>',
  host: { class: 'block min-h-screen' },
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent {}
