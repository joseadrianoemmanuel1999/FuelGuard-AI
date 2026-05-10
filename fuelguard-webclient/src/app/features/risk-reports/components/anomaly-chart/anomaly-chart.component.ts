import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-anomaly-chart',
  standalone: true,
  templateUrl: './anomaly-chart.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AnomalyChartComponent {
  readonly annotation = input('');
}
