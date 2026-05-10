import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/** Preserves the reference histogram palette order (8 columns). */
const BAR_SHELL: readonly string[] = [
  'w-full bg-primary/20 rounded-t border-t border-primary/40',
  'w-full bg-primary/30 rounded-t border-t border-primary/50',
  'w-full bg-primary/40 rounded-t border-t border-primary/60',
  'w-full bg-primary/60 rounded-t border-t border-primary/80',
  'w-full bg-primary/20 rounded-t border-t border-primary/40',
  'w-full bg-primary/30 rounded-t border-t border-primary/50',
  'w-full bg-error/40 rounded-t border-t border-error/60',
  'w-full bg-error/60 rounded-t border-t border-error/80 animate-pulse',
];

@Component({
  selector: 'app-intelligence-chart',
  standalone: true,
  templateUrl: './intelligence-chart.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class IntelligenceChartComponent {
  /** Column heights as % of the chart track (flex-end aligned). */
  readonly barHeightsPercent = input<readonly number[]>([40, 55, 70, 95, 30, 45, 85, 100]);

  protected shellClass(i: number): string {
    return BAR_SHELL[Math.min(i, BAR_SHELL.length - 1)] ?? BAR_SHELL[0];
  }
}
