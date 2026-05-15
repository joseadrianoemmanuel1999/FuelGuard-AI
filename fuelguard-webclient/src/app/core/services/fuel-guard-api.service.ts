import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../tokens/api-base-url.token';
import type {
  AiInvestigationDto,
  AlertDto,
  CommandCenterSimulationResultDto,
  PipelineLogDto,
  RiskAssessmentDto,
  TelemetryDto,
} from '../models/command-center.models';
import type {
  ExecutiveRiskReportDto,
  InvestigationsBoardDto,
  LiveFeedEventDto,
  TeamDirectoryDto,
  TerminalLogLineDto,
} from '../models/portal.models';

@Injectable({ providedIn: 'root' })
export class FuelGuardApiService {
  private readonly http = inject(HttpClient);
  private readonly apiBase = inject(API_BASE_URL);

  simulateFuelSmuggling(): Observable<CommandCenterSimulationResultDto> {
    return this.http.post<CommandCenterSimulationResultDto>(
      `${this.apiBase}/command-center/simulate-fuel-smuggling`,
      {},
    );
  }

  simulateSuspiciousActivity(): Observable<CommandCenterSimulationResultDto> {
    return this.http.post<CommandCenterSimulationResultDto>(
      `${this.apiBase}/command-center/simulate-suspicious-activity`,
      {},
    );
  }

  getCurrentRisk(): Observable<RiskAssessmentDto> {
    return this.http.get<RiskAssessmentDto>(`${this.apiBase}/command-center/current-risk`);
  }

  getPipelineLogs(): Observable<PipelineLogDto[]> {
    return this.http.get<PipelineLogDto[]>(`${this.apiBase}/command-center/pipeline-logs`);
  }

  getAlerts(): Observable<AlertDto[]> {
    return this.http.get<AlertDto[]>(`${this.apiBase}/command-center/alerts`);
  }

  getTelemetry(): Observable<TelemetryDto> {
    return this.http.get<TelemetryDto>(`${this.apiBase}/command-center/telemetry`);
  }

  getAiInvestigation(): Observable<AiInvestigationDto> {
    return this.http.get<AiInvestigationDto>(`${this.apiBase}/command-center/ai-investigation`);
  }

  getLiveFeed(): Observable<LiveFeedEventDto[]> {
    return this.http.get<LiveFeedEventDto[]>(`${this.apiBase}/command-center/live-feed`);
  }

  getInvestigations(): Observable<InvestigationsBoardDto> {
    return this.http.get<InvestigationsBoardDto>(`${this.apiBase}/investigations`);
  }

  getCommandCenterInvestigations(): Observable<InvestigationsBoardDto> {
    return this.http.get<InvestigationsBoardDto>(`${this.apiBase}/command-center/investigations`);
  }

  getLogs(): Observable<TerminalLogLineDto[]> {
    return this.http.get<TerminalLogLineDto[]>(`${this.apiBase}/command-center/terminal-lines`);
  }

  getReports(): Observable<ExecutiveRiskReportDto> {
    return this.http.get<ExecutiveRiskReportDto>(`${this.apiBase}/command-center/reports`);
  }

  getTeamData(): Observable<TeamDirectoryDto> {
    return this.http.get<TeamDirectoryDto>(`${this.apiBase}/command-center/team`);
  }
}
