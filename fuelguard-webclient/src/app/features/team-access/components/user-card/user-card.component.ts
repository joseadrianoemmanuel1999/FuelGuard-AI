import { NgClass } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import type { TeamMemberDto } from '../../../../core/models/portal.models';

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [NgClass],
  templateUrl: './user-card.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserCardComponent {
  readonly member = input.required<TeamMemberDto>();

  protected levelBadgeClass(level: number): string {
    if (level >= 5) return 'bg-tertiary/10 text-tertiary border border-tertiary/20';
    if (level <= 2) return 'bg-outline/10 text-on-surface-variant border border-outline/20';
    return 'bg-tertiary/10 text-tertiary border border-tertiary/20';
  }

  protected dotClass(m: TeamMemberDto): string {
    if (!m.onlineDot) return 'bg-secondary-container';
    return 'bg-tertiary bloom-indicator';
  }
}
