import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { FuelGuardApiService } from './fuel-guard-api.service';
import type {
  AiInvestigationDto,
  AlertDto,
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

const delay = (ms: number): Promise<void> =>
  new Promise((resolve) => {
    window.setTimeout(resolve, ms);
  });

const defaultInvestigation: AiInvestigationDto = {
  summaryTokens: [
    { text: '"Anomaly detected in ', tone: 'default' },
    { text: 'Southern Sector', tone: 'primary' },
    { text: ' pipeline flow. Volume variance exceeds ', tone: 'default' },
    { text: '12% threshold', tone: 'error' },
    { text: '. Patterns suggest ', tone: 'default' },
    { text: 'illegal siphoning', tone: 'error' },
    { text: ' at Node 42. High confidence match with known smuggling vectors."', tone: 'default' },
  ],
  recommendedActions: [
    { title: 'Shut down Valve 42', materialIcon: 'dangerous', accent: 'error' },
    { title: 'Deploy Inspection Drone', materialIcon: 'flight_takeoff', accent: 'tertiary' },
    { title: 'Notify Local Compliance', materialIcon: 'assignment_ind', accent: 'primary' },
  ],
};

const defaultLogs: PipelineLogDto[] = [
  { timestamp: '14:22:01', stage: 'INGESTION', message: 'Data stream initialized from 14 nodes' },
  { timestamp: '14:22:03', stage: 'PROTOCOL', message: 'Handshake verified for Edge-Gateway-04' },
  {
    timestamp: '14:22:05',
    stage: 'ANOMALY',
    message: 'Deviation detected in Node 42 (Delta: +12.4%)',
  },
  { timestamp: '14:22:05', stage: 'RISK', message: 'Escalating to Level 3 Internal Investigation' },
  {
    timestamp: '14:22:08',
    stage: 'AI_CORE',
    message: 'Cross-referencing historical siphoning patterns...',
  },
  {
    timestamp: '14:22:12',
    stage: 'AI_CORE',
    message: 'Match found: Sector Southern-7 Pattern Beta-4',
  },
  { timestamp: '14:22:15', stage: 'SYSTEM', message: 'Visual feed from Drone-A9 engaged' },
  {
    timestamp: '14:22:18',
    stage: 'ALERT',
    message: 'Security personnel dispatched to Node 42 coordinates',
  },
];

@Injectable({ providedIn: 'root' })
export class DashboardStateService {
  private readonly api = inject(FuelGuardApiService);

  private streamSession = 0;

  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly risk = signal<RiskAssessmentDto>({
    score: 87,
    level: 'HIGH',
    levelLabel: 'Risk Level: HIGH',
    summary: 'Synthetic posture until API hydrates.',
  });
  readonly investigation = signal<AiInvestigationDto>(defaultInvestigation);
  readonly logs = signal<PipelineLogDto[]>(defaultLogs);
  readonly alerts = signal<AlertDto[]>([]);
  readonly telemetry = signal<TelemetryDto>({
    flowRateLitersPerSecond: 412.5,
    pressureBar: 14.2,
    temperatureCelsius: 24.5,
    statusLabel: 'CRITICAL DEVIATION',
    panelTitle: 'Node 42 Telemetry',
    mapBadgeText: 'ACTIVE ALERT: SECTOR 7-G',
  });
  readonly activePipelineStage = signal(3);
  readonly alertStateActive = signal(true);
  readonly cpuLoadPercent = signal(42);
  readonly investigationRevision = signal(0);

  readonly liveFeed = signal<LiveFeedEventDto[]>([]);
  readonly investigations = signal<InvestigationsBoardDto | null>(null);
  readonly reports = signal<ExecutiveRiskReportDto | null>(null);
  readonly team = signal<TeamDirectoryDto | null>(null);
  readonly terminalLines = signal<TerminalLogLineDto[]>([]);

  readonly aiSummary = computed(() =>
    this.investigation()
      .summaryTokens.map((t) => t.text)
      .join(''),
  );

  readonly mobileSidebarOpen = signal(false);

  setMobileSidebar(open: boolean): void {
    this.mobileSidebarOpen.set(open);
  }

  async refresh(): Promise<void> {
    if (this.loading()) return;
    this.loading.set(true);
    this.errorMessage.set(null);
    try {
      const [
        risk,
        logs,
        alerts,
        telemetry,
        investigation,
        liveFeed,
        investigations,
        reports,
        team,
        terminalLines,
      ] = await Promise.all([
        firstValueFrom(this.api.getCurrentRisk()),
        firstValueFrom(this.api.getPipelineLogs()),
        firstValueFrom(this.api.getAlerts()),
        firstValueFrom(this.api.getTelemetry()),
        firstValueFrom(this.api.getAiInvestigation()),
        firstValueFrom(this.api.getLiveFeed()),
        firstValueFrom(this.api.getCommandCenterInvestigations()),
        firstValueFrom(this.api.getReports()),
        firstValueFrom(this.api.getTeamData()),
        firstValueFrom(this.api.getLogs()),
      ]);
      this.risk.set(risk);
      this.logs.set(logs);
      this.alerts.set(alerts);
      this.telemetry.set(telemetry);
      this.investigation.set(investigation);
      this.liveFeed.set(liveFeed);
      this.investigations.set(investigations);
      this.reports.set(reports);
      this.team.set(team);
      this.terminalLines.set(terminalLines);
      this.investigationRevision.update((v) => v + 1);
    } catch (err) {
      console.warn('[Dashboard] refresh failed — keeping current UI state', err);
      this.errorMessage.set('Unable to reach command center API. Showing cached layout.');
    } finally {
      this.loading.set(false);
    }
  }

  async simulateFuelSmugglingScenario(): Promise<void> {
    if (this.loading()) return;
    this.loading.set(true);
    this.errorMessage.set(null);
    const session = ++this.streamSession;

    const anim = this.animatePipeline();
    try {
      await firstValueFrom(this.api.simulateFuelSmuggling());

      const [
        risk,
        pipelineLogs,
        investigation,
        alerts,
        telemetry,
        liveFeed,
        investigations,
      ] = await Promise.all([
        firstValueFrom(this.api.getCurrentRisk()),
        firstValueFrom(this.api.getPipelineLogs()),
        firstValueFrom(this.api.getAiInvestigation()),
        firstValueFrom(this.api.getAlerts()),
        firstValueFrom(this.api.getTelemetry()),
        firstValueFrom(this.api.getLiveFeed()),
        firstValueFrom(this.api.getCommandCenterInvestigations()),
      ]);

      this.risk.set(risk);
      this.investigation.set(investigation);
      this.alerts.set(alerts);
      this.telemetry.set(telemetry);
      this.liveFeed.set(liveFeed);
      this.investigations.set(investigations);
      this.investigationRevision.update((v) => v + 1);
      this.alertStateActive.set(true);
      this.activePipelineStage.set(4);
      this.cpuLoadPercent.set(Math.min(99, this.cpuLoadPercent() + 6));

      const injectEvent: LiveFeedEventDto = {
        timestamp: `[${new Date().toLocaleTimeString('en-GB', { hour12: false })}]`,
        category: 'ALERT',
        title: 'Fuel smuggling scenario injected',
        body: 'Simulation pipeline completed; cross-correlation with live intelligence feed.',
        tags: ['SIMULATION', `Risk ${risk.score}`],
        accent: 'error',
      };
      this.liveFeed.update((cur) => [injectEvent, ...cur]);

      await this.streamLogsProgressive(pipelineLogs, 340, session);
    } catch (err) {
      console.error('[Dashboard] simulate failed', err);
      this.errorMessage.set('Simulation or sync failed. Previous data retained.');
    } finally {
      await anim;
      this.loading.set(false);
    }
  }

  /** @deprecated use simulateFuelSmugglingScenario */
  async simulate(): Promise<void> {
    return this.simulateFuelSmugglingScenario();
  }

  async animatePipeline(): Promise<void> {
    const stepMs = 420;
    for (let stage = 0; stage <= 4; stage++) {
      this.activePipelineStage.set(stage);
      await delay(stepMs);
    }
  }

  private async streamLogsProgressive(
    entries: PipelineLogDto[],
    lineDelayMs: number,
    session: number,
  ): Promise<void> {
    this.logs.set([]);
    for (const entry of entries) {
      if (session !== this.streamSession) return;
      this.logs.update((cur) => [...cur, entry]);
      await delay(lineDelayMs);
    }
  }
}
