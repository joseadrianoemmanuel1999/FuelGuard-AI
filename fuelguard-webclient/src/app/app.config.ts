import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { API_BASE_URL } from './core/tokens/api-base-url.token';
import { environment } from '../environments/environment';
import { correlationIdInterceptor } from './core/interceptors/correlation-id.interceptor';
import { loggingInterceptor } from './core/interceptors/logging.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([correlationIdInterceptor, loggingInterceptor, errorInterceptor]),
    ),
    provideAnimations(),
    { provide: API_BASE_URL, useValue: environment.apiBaseUrl },
  ],
};
