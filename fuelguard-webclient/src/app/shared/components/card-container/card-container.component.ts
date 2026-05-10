import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-card-container',
  standalone: true,
  templateUrl: './card-container.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CardContainerComponent {
  /** Extra classes for the outer panel (e.g. glass-panel rounded-lg) */
  readonly panelClass = input('glass-panel rounded-lg');
}
