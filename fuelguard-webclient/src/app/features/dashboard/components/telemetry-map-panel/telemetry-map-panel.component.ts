import {
  ChangeDetectionStrategy,
  Component,
  input,
} from '@angular/core';
import { DecimalPipe, NgClass } from '@angular/common';
import type { TelemetryDto } from '../../../../core/models/command-center.models';

@Component({
  selector: 'app-telemetry-map-panel',
  standalone: true,
  imports: [NgClass, DecimalPipe],
  templateUrl: './telemetry-map-panel.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TelemetryMapPanelComponent {
  readonly telemetry = input.required<TelemetryDto>();
  readonly alertActive = input(true);
}
