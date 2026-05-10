import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-confidence-bar',
  standalone: true,
  templateUrl: './confidence-bar.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfidenceBarComponent {
  readonly percent = input(0);
  /** Tailwind width class for fill e.g. bg-primary */
  readonly fillClass = input('bg-primary');
  readonly labelClass = input('text-primary');

  protected readonly widthStyle = computed(() => `${Math.max(0, Math.min(100, this.percent()))}%`);
}
