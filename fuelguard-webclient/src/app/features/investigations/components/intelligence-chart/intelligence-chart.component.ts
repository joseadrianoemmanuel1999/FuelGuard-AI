import { ChangeDetectionStrategy, Component, input } from '@angular/core';

const DEFAULT_HISTOGRAM_BAR_HEIGHT_PERCENTS = [40, 55, 70, 95, 30, 45, 85, 100] as const satisfies readonly number[];

const HISTOGRAM_BAR_COLUMN_SHELL_CLASSES = [
  'w-full bg-primary/20 rounded-t border-t border-primary/40',
  'w-full bg-primary/30 rounded-t border-t border-primary/50',
  'w-full bg-primary/40 rounded-t border-t border-primary/60',
  'w-full bg-primary/60 rounded-t border-t border-primary/80',
  'w-full bg-primary/20 rounded-t border-t border-primary/40',
  'w-full bg-primary/30 rounded-t border-t border-primary/50',
  'w-full bg-error/40 rounded-t border-t border-error/60',
  'w-full bg-error/60 rounded-t border-t border-error/80 animate-pulse',
] as const;

type HistogramBarColumnShellClass = (typeof HISTOGRAM_BAR_COLUMN_SHELL_CLASSES)[number];

@Component({
  selector: 'app-intelligence-chart',
  standalone: true,
  templateUrl: './intelligence-chart.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class IntelligenceChartComponent {
  readonly histogramBarHeightPercents = input<readonly number[]>([
    ...DEFAULT_HISTOGRAM_BAR_HEIGHT_PERCENTS,
  ]);

  protected shellClassForHistogramColumn(columnIndex: number): HistogramBarColumnShellClass {
    const lastIndex = HISTOGRAM_BAR_COLUMN_SHELL_CLASSES.length - 1;
    const clampedColumnIndex = Math.min(Math.max(columnIndex, 0), lastIndex);
    return HISTOGRAM_BAR_COLUMN_SHELL_CLASSES[clampedColumnIndex];
  }
}
