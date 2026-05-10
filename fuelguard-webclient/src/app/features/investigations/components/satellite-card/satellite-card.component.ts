import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-satellite-card',
  standalone: true,
  templateUrl: './satellite-card.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SatelliteCardComponent {
  readonly imageUrl = input(
    'https://lh3.googleusercontent.com/aida-public/AB6AXuAeabevNiMjW--b_zRoyeYtqQRlF9VO0VnqKX8wwVTtl0Fy9dSAUWTL8xkqUSO2tD7tsUHy53cbry4JFhmoPJC4GlWwFNbpO1uezDCzcN8z1-N3-TgL1pIcH-xwLnQ5kuVDSN3S1E6jTXNNqWIz3ltjtg-tujSCggjOXP2lF7QGYELuL48bYjxT-UEHckD_anBTCZPWxauK1CyJl50FKZsdoJochmlVeZwIAe--qDcNehkS41FuAjotm9vFIn6gOMp7xsD6LIu70mFA',
  );
  readonly imageAlt = input('Cyber Surveillance');
  readonly title = input('Satellite Overlay');
  readonly description = input('');
}
