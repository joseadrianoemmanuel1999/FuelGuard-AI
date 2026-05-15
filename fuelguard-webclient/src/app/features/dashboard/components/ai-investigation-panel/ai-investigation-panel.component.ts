import {
  ChangeDetectionStrategy,
  Component,
  input,
} from '@angular/core';
import { NgClass } from '@angular/common';
import type { AiInvestigationDto } from '../../../../core/models/command-center.models';

@Component({
  selector: 'app-ai-investigation-panel',
  standalone: true,
  imports: [NgClass],
  templateUrl: './ai-investigation-panel.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AiInvestigationPanelComponent {
  readonly investigation = input.required<AiInvestigationDto>();
  readonly revision = input(0);
}
