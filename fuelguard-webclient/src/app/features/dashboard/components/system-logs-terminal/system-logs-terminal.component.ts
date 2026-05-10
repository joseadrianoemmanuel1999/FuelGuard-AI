import {
  ChangeDetectionStrategy,
  Component,
  effect,
  ElementRef,
  input,
  viewChild,
} from '@angular/core';
import { NgClass } from '@angular/common';
import type { PipelineLogDto } from '../../../../core/models/command-center.models';
import { terminalLineEnter } from '../../dashboard.animations';

@Component({
  selector: 'app-system-logs-terminal',
  standalone: true,
  imports: [NgClass],
  templateUrl: './system-logs-terminal.component.html',
  animations: [terminalLineEnter],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SystemLogsTerminalComponent {
  readonly logs = input.required<PipelineLogDto[]>();

  private readonly viewport = viewChild<ElementRef<HTMLElement>>('scrollViewport');

  constructor() {
    effect(() => {
      this.logs();
      queueMicrotask(() => this.scrollToBottom());
    });
  }

  private scrollToBottom(): void {
    const el = this.viewport()?.nativeElement;
    if (!el) return;
    el.scrollTo({ top: el.scrollHeight, behavior: 'smooth' });
  }

  trackLog(_index: number, log: PipelineLogDto): string {
    return `${log.timestamp}|${log.stage}|${log.message}`;
  }

  stageClass(stage: string): Record<string, boolean> {
    const s = stage.toUpperCase();
    return {
      'text-tertiary': s === 'INGESTION',
      'text-on-surface-variant': s === 'PROTOCOL' || s === 'SYSTEM',
      'text-error': s === 'ANOMALY' || s === 'RISK' || s === 'ALERT',
      'text-primary': s === 'AI_CORE',
    };
  }
}
