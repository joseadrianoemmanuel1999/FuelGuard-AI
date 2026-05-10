import {
  animate,
  keyframes,
  style,
  transition,
  trigger,
} from '@angular/animations';

export const fadeInSummary = trigger('fadeInSummary', [
  transition(':enter', [
    style({ opacity: 0, transform: 'translateY(8px)' }),
    animate(
      '480ms cubic-bezier(0.22, 1, 0.36, 1)',
      style({ opacity: 1, transform: 'translateY(0)' }),
    ),
  ]),
  transition('* => *', [
    style({ opacity: 0.72, transform: 'translateY(4px)' }),
    animate(
      '420ms cubic-bezier(0.22, 1, 0.36, 1)',
      style({ opacity: 1, transform: 'translateY(0)' }),
    ),
  ]),
]);

export const terminalLineEnter = trigger('terminalLineEnter', [
  transition(':enter', [
    style({ opacity: 0, transform: 'translateY(6px)' }),
    animate(
      '320ms cubic-bezier(0.33, 1, 0.68, 1)',
      style({ opacity: 1, transform: 'translateY(0)' }),
    ),
  ]),
]);

export const riskCardPulse = trigger('riskCardPulse', [
  transition('* => *', [
    animate(
      '520ms ease-out',
      keyframes([
        style({
          boxShadow: '0 0 0 0 rgba(255, 180, 171, 0)',
          offset: 0,
        }),
        style({
          boxShadow: '0 0 28px 3px rgba(255, 180, 171, 0.28)',
          offset: 0.45,
        }),
        style({
          boxShadow: 'inset 0 0 0 1px rgba(255, 255, 255, 0.05)',
          offset: 1,
        }),
      ]),
    ),
  ]),
]);
