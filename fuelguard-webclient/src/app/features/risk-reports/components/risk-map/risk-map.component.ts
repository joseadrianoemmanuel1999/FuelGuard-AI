import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import type { PipelineSectorDto } from '../../../../core/models/portal.models';

@Component({
  selector: 'app-risk-map',
  standalone: true,
  templateUrl: './risk-map.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RiskMapComponent {
  readonly mapImageUrl = input('');
  readonly sectors = input<PipelineSectorDto[]>([]);

  protected chipClass(tone: string): string {
    if (tone === 'error') return 'bg-error-container text-on-error-container';
    if (tone === 'primary') return 'bg-secondary-container/40 text-on-surface-variant';
    if (tone === 'muted') return 'bg-surface-container-highest text-on-surface-variant';
    return 'bg-secondary-container/40 text-on-surface-variant';
  }

  protected footerClass(tone: string): string {
    if (tone === 'error') return 'text-error';
    if (tone === 'tertiary') return 'text-tertiary';
    if (tone === 'primary') return 'text-primary';
    return 'text-on-surface-variant';
  }
}
