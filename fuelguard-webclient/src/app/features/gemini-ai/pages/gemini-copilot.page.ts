import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  viewChild,
} from '@angular/core';
import { DashboardChromeComponent } from '../../../layout/dashboard-chrome.component';
import { GeminiChatPanelComponent } from '../components/gemini-chat-panel/gemini-chat-panel.component';
import { GeminiInsightsPanelComponent } from '../components/gemini-insights-panel/gemini-insights-panel.component';
import { GeminiChatStateService } from '../services/gemini-chat-state.service';

@Component({
  selector: 'app-gemini-copilot-page',
  standalone: true,
  imports: [DashboardChromeComponent, GeminiChatPanelComponent, GeminiInsightsPanelComponent],
  templateUrl: './gemini-copilot.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GeminiCopilotPage implements OnInit, AfterViewInit {
  protected readonly state = inject(GeminiChatStateService);
  private readonly chatPanel = viewChild(GeminiChatPanelComponent);

  ngOnInit(): void {
    void this.state.loadInsights();
  }

  ngAfterViewInit(): void {
    this.chatPanel()?.scrollToBottom();
  }

  protected onSend(payload: { question: string; stationHint: string; periodHint: string }): void {
    void this.state.ask(payload.question, payload.stationHint, payload.periodHint, () => {
      this.chatPanel()?.scrollToBottom();
    });
  }
}
