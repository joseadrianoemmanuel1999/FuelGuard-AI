import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  input,
  output,
  viewChild,
} from '@angular/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardContainerComponent } from '../../../../shared/components/card-container/card-container.component';
import { SimpleMarkdownPipe } from '../../pipes/simple-markdown.pipe';
import type { GeminiChatMessage } from '../../models/gemini.models';

@Component({
  selector: 'app-gemini-chat-panel',
  standalone: true,
  imports: [NgClass, FormsModule, CardContainerComponent, SimpleMarkdownPipe],
  templateUrl: './gemini-chat-panel.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GeminiChatPanelComponent {
  readonly messages = input<GeminiChatMessage[]>([]);
  readonly loading = input(false);
  readonly error = input<string | null>(null);

  readonly sendQuestion = output<{ question: string; stationHint: string; periodHint: string }>();

  protected question = '';
  protected stationHint = '';
  protected periodHint = '30d';

  private readonly scrollHost = viewChild<ElementRef<HTMLElement>>('scrollHost');

  protected readonly suggestions = [
    'Qual posto apresenta maior risco operacional?',
    'Existe comportamento suspeito nas últimas transações?',
    'Resume os principais riscos e anomalias.',
    'Quais ações recomendas para investigação imediata?',
  ];

  protected submit(): void {
    const q = this.question.trim();
    if (!q || this.loading()) return;
    this.sendQuestion.emit({
      question: q,
      stationHint: this.stationHint.trim(),
      periodHint: this.periodHint.trim() || '30d',
    });
    this.question = '';
  }

  protected useSuggestion(text: string): void {
    this.question = text;
    this.submit();
  }

  protected riskBadgeClass(level: string | undefined): string {
    const l = (level ?? 'UNKNOWN').toUpperCase();
    if (l === 'CRITICAL' || l === 'HIGH') return 'bg-error-container/30 text-on-error-container border-error/40';
    if (l === 'MEDIUM') return 'bg-tertiary-container/30 text-on-tertiary-container border-tertiary/40';
    return 'bg-primary/15 text-primary border-primary/30';
  }

  scrollToBottom(): void {
    const el = this.scrollHost()?.nativeElement;
    if (el) el.scrollTop = el.scrollHeight;
  }
}
