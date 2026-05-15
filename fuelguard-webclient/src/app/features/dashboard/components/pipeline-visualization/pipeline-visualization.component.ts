import { NgClass } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  input,
} from '@angular/core';

import {
  PIPELINE_STAGES,
  type PipelineStage,
} from '../../../../core/models/command-center.models';

@Component({
  selector: 'app-pipeline-visualization',
  standalone: true,
  imports: [NgClass],
  templateUrl: './pipeline-visualization.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PipelineVisualizationComponent {
  readonly activeStage = input<PipelineStage>(PIPELINE_STAGES[3]);
  readonly cpuLoadPercent = input<number>(42);
  readonly throughputWidthClass = input<string>('w-3/4');
}
