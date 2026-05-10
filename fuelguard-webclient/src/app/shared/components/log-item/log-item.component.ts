import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import type { TerminalLogLineDto } from '../../../core/models/portal.models';

@Component({
  selector: 'app-log-item',
  standalone: true,
  templateUrl: './log-item.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogItemComponent {
  readonly line = input.required<TerminalLogLineDto>();

  protected readonly rowClass = computed(() => {
    const v = this.line().rowVariant;
    const base = 'flex gap-stack-md group transition-colors font-data-tabular text-data-tabular';
    switch (v) {
      case 'warn':
        return `${base} hover:bg-surface-container/30 bg-secondary-container/10`;
      case 'crit':
        return `${base} hover:bg-surface-container/30 bg-error-container/20 border-l-2 border-error`;
      case 'info-dim':
        return `${base} opacity-70`;
      default:
        return `${base} hover:bg-surface-container/30`;
    }
  });

  protected readonly levelClass = computed(() => {
    const level = this.line().level;
    if (level === 'WARN') return 'text-[#FFAB40]';
    if (level === 'CRIT') return 'text-error';
    return 'text-primary';
  });

  protected readonly categoryClass = computed(() => {
    const level = this.line().level;
    if (level === 'WARN') return 'text-[#FFAB40]';
    if (level === 'CRIT') return 'text-error';
    return 'text-tertiary-fixed';
  });

  protected readonly messageClass = computed(() => {
    if (this.line().level === 'CRIT') return 'text-on-error-container flex-1 font-bold';
    if (this.line().level === 'WARN') return 'text-on-surface flex-1';
    return 'text-on-surface-variant flex-1';
  });
}
