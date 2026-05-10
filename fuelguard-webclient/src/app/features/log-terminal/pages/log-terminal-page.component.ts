import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, OnInit, signal } from '@angular/core';
import type { TerminalLogLineDto } from '../../../core/models/portal.models';
import { DashboardStateService } from '../../../core/services/dashboard-state.service';
import { FilterBarComponent, type TerminalSeverityFilter } from '../components/filter-bar/filter-bar.component';
import { TerminalLogViewComponent } from '../components/terminal-log-view/terminal-log-view.component';

@Component({
  selector: 'app-log-terminal-page',
  standalone: true,
  imports: [DatePipe, FilterBarComponent, TerminalLogViewComponent],
  templateUrl: './log-terminal-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogTerminalPageComponent implements OnInit {
  protected readonly state = inject(DashboardStateService);
  protected readonly severity = signal<TerminalSeverityFilter>('all');
  protected readonly cleared = signal(false);
  protected readonly lastRefresh = signal(new Date());

  protected readonly filteredLines = computed((): TerminalLogLineDto[] => {
    if (this.cleared()) return [];
    const raw = this.state.terminalLines();
    const f = this.severity();
    if (f === 'all') return raw;
    if (f === 'warning') return raw.filter((l) => l.level === 'WARN');
    return raw.filter((l) => l.level === 'CRIT');
  });

  ngOnInit(): void {
    void this.state.refresh().then(() => this.lastRefresh.set(new Date()));
  }

  protected onExport(): void {
    const blob = new Blob(
      [this.filteredLines().map((l) => `${l.timestamp} ${l.level} ${l.category} ${l.message}`).join('\n')],
      { type: 'text/plain' },
    );
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'fuelguard-terminal.log';
    a.click();
    URL.revokeObjectURL(url);
  }

  protected onClear(): void {
    this.cleared.set(true);
  }
}
