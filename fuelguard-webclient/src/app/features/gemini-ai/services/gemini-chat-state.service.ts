import { inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import type { GeminiChatMessage, GeminiInsightsResponseDto } from '../models/gemini.models';
import { GeminiApiService } from './gemini-api.service';

@Injectable({ providedIn: 'root' })
export class GeminiChatStateService {
  private readonly api = inject(GeminiApiService);

  readonly messages = signal<GeminiChatMessage[]>([]);
  readonly insights = signal<GeminiInsightsResponseDto | null>(null);
  readonly chatLoading = signal(false);
  readonly insightsLoading = signal(false);
  readonly chatError = signal<string | null>(null);
  readonly insightsError = signal<string | null>(null);

  async loadInsights(): Promise<void> {
    this.insightsLoading.set(true);
    this.insightsError.set(null);
    try {
      const data = await firstValueFrom(this.api.getInsights());
      this.insights.set(data);
    } catch {
      this.insightsError.set('Unable to load AI insights. Check API connectivity.');
    } finally {
      this.insightsLoading.set(false);
    }
  }

  async ask(
    question: string,
    stationHint: string,
    periodHint: string,
    onTyped?: () => void,
  ): Promise<void> {
    const userMsg: GeminiChatMessage = {
      id: crypto.randomUUID(),
      role: 'user',
      content: question,
    };
    this.messages.update((m) => [...m, userMsg]);
    this.chatLoading.set(true);
    this.chatError.set(null);

    const typingId = crypto.randomUUID();
    this.messages.update((m) => [
      ...m,
      { id: typingId, role: 'assistant', content: '', isTyping: true },
    ]);

    try {
      const res = await firstValueFrom(
        this.api.chat({
          question,
          stationHint: stationHint || null,
          periodHint: periodHint || null,
        }),
      );

      this.messages.update((m) => m.filter((x) => x.id !== typingId));

      const fullText = res.disclaimer ? `${res.answer}\n\n_${res.disclaimer}_` : res.answer;
      const assistantId = crypto.randomUUID();
      this.messages.update((m) => [
        ...m,
        {
          id: assistantId,
          role: 'assistant',
          content: '',
          riskLevel: res.riskLevel,
          recommendations: res.recommendations,
          explainability: res.explainability,
          poweredByGemini: res.poweredByGemini,
          isTyping: true,
        },
      ]);

      await this.typeOut(assistantId, fullText, onTyped);
    } catch {
      this.messages.update((m) => m.filter((x) => x.id !== typingId));
      this.chatError.set('Intelligence service unavailable. Deterministic summary will apply when the API recovers.');
    } finally {
      this.chatLoading.set(false);
    }
  }

  private async typeOut(id: string, full: string, onTick?: () => void): Promise<void> {
    const chunk = Math.max(2, Math.floor(full.length / 80));
    for (let i = chunk; i <= full.length; i += chunk) {
      const slice = full.slice(0, i);
      this.messages.update((list) =>
        list.map((m) => (m.id === id ? { ...m, content: slice, isTyping: i < full.length } : m)),
      );
      onTick?.();
      await delay(16);
    }
    this.messages.update((list) =>
      list.map((m) => (m.id === id ? { ...m, content: full, isTyping: false } : m)),
    );
    onTick?.();
  }
}

function delay(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}
