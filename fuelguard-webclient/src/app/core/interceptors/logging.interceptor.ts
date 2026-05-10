import { HttpInterceptorFn } from '@angular/common/http';
import { tap } from 'rxjs/operators';

export const loggingInterceptor: HttpInterceptorFn = (req, next) => {
  const started = performance.now();
  return next(req).pipe(
    tap({
      next: () => {
        if (!req.url.includes('command-center') && !req.url.includes('/api')) return;
        const ms = (performance.now() - started).toFixed(0);
        console.info(`[FuelGuard HTTP] ${req.method} ${req.url} ${ms}ms`);
      },
      error: (err: unknown) =>
        console.error(`[FuelGuard HTTP] ${req.method} ${req.url} failed`, err),
    }),
  );
};
