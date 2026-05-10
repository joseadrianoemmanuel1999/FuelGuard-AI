import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) =>
  next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      const payload = err.error;
      const message =
        typeof payload === 'object' && payload && 'error' in payload
          ? String((payload as { error: string }).error)
          : err.message;
      console.error('[FuelGuard API]', err.status, message);
      return throwError(() => err);
    }),
  );
