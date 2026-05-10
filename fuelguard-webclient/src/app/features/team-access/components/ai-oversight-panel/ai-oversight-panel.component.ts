import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import type { AccessLedgerEntryDto, AiOversightPanelDto } from '../../../../core/models/portal.models';

@Component({
  selector: 'app-ai-oversight-panel',
  standalone: true,
  templateUrl: './ai-oversight-panel.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AiOversightPanelComponent {
  readonly oversight = input.required<AiOversightPanelDto>();
  readonly ledger = input<AccessLedgerEntryDto[]>([]);

  protected ledgerTimeClass(tone: string): string {
    if (tone === 'tertiary') return 'text-tertiary';
    if (tone === 'primary') return 'text-primary';
    return 'text-on-surface-variant';
  }
}
