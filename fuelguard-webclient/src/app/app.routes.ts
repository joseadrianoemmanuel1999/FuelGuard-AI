import { Routes } from '@angular/router';
import { AppLayoutComponent } from './layout/app-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: AppLayoutComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'command-center' },
      {
        path: 'command-center',
        loadComponent: () =>
          import('./features/command-center/pages/command-center-page.component').then(
            (m) => m.CommandCenterPageComponent,
          ),
      },
      {
        path: 'live-feed',
        loadComponent: () =>
          import('./features/live-feed/pages/live-feed-page.component').then((m) => m.LiveFeedPageComponent),
      },
      {
        path: 'investigations',
        loadComponent: () =>
          import('./features/investigations/pages/investigations.page').then((m) => m.InvestigationsPage),
      },
      {
        path: 'log-terminal',
        data: { searchPlaceholder: 'Global search... [⌘K]' },
        loadComponent: () =>
          import('./features/log-terminal/pages/log-terminal-page.component').then(
            (m) => m.LogTerminalPageComponent,
          ),
      },
      {
        path: 'risk-reports',
        data: { searchPlaceholder: 'GLOBAL VIGILANCE SEARCH' },
        loadComponent: () =>
          import('./features/risk-reports/pages/risk-reports-page.component').then(
            (m) => m.RiskReportsPageComponent,
          ),
      },
      {
        path: 'team-access',
        data: { searchPlaceholder: 'Personnel search (Cmd+K)' },
        loadComponent: () =>
          import('./features/team-access/pages/team-access-page.component').then(
            (m) => m.TeamAccessPageComponent,
          ),
      },
    ],
  },
  { path: '**', pathMatch: 'full', redirectTo: '/command-center' },
];
