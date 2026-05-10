import { NgClass } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  input,
} from '@angular/core';

@Component({
  selector: 'app-pipeline-visualization',
  standalone: true,
  imports: [NgClass],
  templateUrl: './pipeline-visualization.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PipelineVisualizationComponent {
  /** 0–4: Ingestion → Aggregation → Anomaly → Risk → Alert */
  readonly activeStage = input(3);
  readonly cpuLoadPercent = input(42);
  readonly throughputWidthClass = input('w-3/4');
}
