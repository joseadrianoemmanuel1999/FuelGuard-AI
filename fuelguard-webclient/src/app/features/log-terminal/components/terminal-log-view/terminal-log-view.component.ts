import {
  ChangeDetectionStrategy,
  Component,
  effect,
  ElementRef,
  input,
  untracked,
  viewChild,
} from '@angular/core';
import type { TerminalLogLineDto } from '../../../../core/models/portal.models';
import { LogItemComponent } from '../../../../shared/components/log-item/log-item.component';

@Component({
  selector: 'app-terminal-log-view',
  standalone: true,
  imports: [LogItemComponent],
  templateUrl: './terminal-log-view.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TerminalLogViewComponent {
  private readonly host = viewChild<ElementRef<HTMLElement>>('scrollHost');

  readonly lines = input<TerminalLogLineDto[]>([]);

  constructor() {
    effect(() => {
      this.lines().length;
      untracked(() => {
        queueMicrotask(() => {
          const el = this.host()?.nativeElement;
          if (el) el.scrollTop = el.scrollHeight;
        });
      });
    });
  }
}
