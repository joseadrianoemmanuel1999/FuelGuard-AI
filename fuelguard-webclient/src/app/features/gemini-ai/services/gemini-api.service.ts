import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/tokens/api-base-url.token';
import type {
  GeminiChatRequestDto,
  GeminiChatResponseDto,
  GeminiInsightsResponseDto,
} from '../models/gemini.models';

@Injectable({ providedIn: 'root' })
export class GeminiApiService {
  private readonly http = inject(HttpClient);
  private readonly apiBase = inject(API_BASE_URL);

  chat(body: GeminiChatRequestDto): Observable<GeminiChatResponseDto> {
    return this.http.post<GeminiChatResponseDto>(`${this.apiBase}/gemini/chat`, body);
  }

  getInsights(): Observable<GeminiInsightsResponseDto> {
    return this.http.get<GeminiInsightsResponseDto>(`${this.apiBase}/gemini/insights`);
  }
}
