import {
  afterNextRender,
  ChangeDetectionStrategy,
  Component,
  effect,
  ElementRef,
  inject,
  Injector,
  input,
  viewChild,
} from '@angular/core';
import { NgClass } from '@angular/common';
import type { PipelineLogDto } from '../../../../core/models/command-center.models';

type LogStageTextClassName =
  | 'text-tertiary'
  | 'text-on-surface-variant'
  | 'text-error'
  | 'text-primary';

type LogStageNgClassMap = Partial<Record<LogStageTextClassName, boolean>>;

@Component({
  selector: 'app-system-logs-terminal',
  standalone: true,
  imports: [NgClass],
  templateUrl: './system-logs-terminal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SystemLogsTerminalComponent {
  readonly logs = input.required<PipelineLogDto[]>();

  private readonly injector = inject(Injector);
  private readonly scrollContainerRef =
    viewChild<ElementRef<HTMLElement>>('logsScrollContainer');

  private readonly syncScrollAfterLogsChange = effect(() => {
    this.logs();
    afterNextRender(() => this.scrollLogContainerToBottom(), {
      injector: this.injector,
    });
  });

  trackByLogEntry(_index: number, log: PipelineLogDto): string {
    return `${log.timestamp}|${log.stage}|${log.message}`;
  }

  logStageNgClassMap(logStage: PipelineLogDto['stage']): LogStageNgClassMap {
    const normalized = logStage.trim().toUpperCase();
    return {
      'text-tertiary': normalized === 'INGESTION',
      'text-on-surface-variant':
        normalized === 'PROTOCOL' || normalized === 'SYSTEM',
      'text-error':
        normalized === 'ANOMALY' ||
        normalized === 'RISK' ||
        normalized === 'ALERT',
      'text-primary': normalized === 'AI_CORE',
    };
  }

  private scrollLogContainerToBottom(): void {
    const host = this.scrollContainerRef()?.nativeElement;
    if (!host) {
      return;
    }
    host.scrollTo({ top: host.scrollHeight, behavior: 'smooth' });
  }
}
