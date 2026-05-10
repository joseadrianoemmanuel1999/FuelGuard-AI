import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import type { Investigation } from '../../models/investigation.models';
import { InvestigationRowComponent } from '../investigation-row/investigation-row.component';

@Component({
  selector: 'app-investigations-table',
  standalone: true,
  imports: [InvestigationRowComponent],
  templateUrl: './investigations-table.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InvestigationsTableComponent {
  readonly investigations = input<Investigation[]>([]);
  readonly selectedId = input<string | null>(null);
  readonly select = output<string>();
}
