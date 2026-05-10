import { ChangeDetectionStrategy, Component, model, output } from '@angular/core';

export type TerminalSeverityFilter = 'all' | 'warning' | 'critical';

@Component({
  selector: 'app-filter-bar',
  standalone: true,
  templateUrl: './filter-bar.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FilterBarComponent {
  readonly severity = model<TerminalSeverityFilter>('all');
  readonly liveMode = model(true);
  readonly exportClick = output<void>();
  readonly clearClick = output<void>();
}
