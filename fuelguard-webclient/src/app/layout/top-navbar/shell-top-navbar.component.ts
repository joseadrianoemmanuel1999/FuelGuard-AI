import { ChangeDetectionStrategy, Component, inject, input, output } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, map, startWith } from 'rxjs';

@Component({
  selector: 'app-shell-top-navbar',
  standalone: true,
  templateUrl: './shell-top-navbar.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ShellTopNavbarComponent {
  private readonly router = inject(Router);

  readonly loading = input(false);
  readonly menuToggle = output<void>();

  /** Resolved from deepest child route `data['searchPlaceholder']`. */
  protected readonly searchPlaceholder = toSignal(
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map(() => this.resolveSearchPlaceholder()),
      startWith(this.resolveSearchPlaceholder()),
    ),
    { initialValue: 'Search protocol...' },
  );

  private resolveSearchPlaceholder(): string {
    let current: ActivatedRoute | null = this.router.routerState.root;
    while (current?.firstChild) {
      current = current.firstChild;
    }
    const raw = current?.snapshot?.data?.['searchPlaceholder'];
    return typeof raw === 'string' ? raw : 'Search protocol...';
  }
}
