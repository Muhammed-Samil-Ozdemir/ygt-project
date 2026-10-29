import { ChangeDetectionStrategy, Component, computed, input, ViewEncapsulation } from '@angular/core';

/** 24x24 çizgi simgeler. Her simge bir ya da birkaç SVG path'inden oluşur. */
const ICONS = {
  menu: ['M4 6h16M4 12h16M4 18h16'],
  x: ['M6 6l12 12M18 6 6 18'],
  check: ['M5 12.5l4.5 4.5L19 7.5'],
  plus: ['M12 5v14M5 12h14'],
  'arrow-up': ['M12 19V5M6 11l6-6 6 6'],
  'arrow-down': ['M12 5v14M18 13l-6 6-6-6'],
  'arrow-right': ['M5 12h14M13 6l6 6-6 6'],
  'chevron-right': ['M9 6l6 6-6 6'],
  external: ['M14 4h6v6M20 4l-9 9', 'M18 14v5a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1V7a1 1 0 0 1 1-1h5'],
  trash: ['M4 7h16M10 11v6M14 11v6', 'M6 7l1 12a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2l1-12M9 7V4h6v3'],
  edit: ['M4 20h4L19 9l-4-4L4 16v4z', 'M14 6l4 4'],
  eye: ['M2 12s3.6-7 10-7 10 7 10 7-3.6 7-10 7S2 12 2 12z', 'M12 9a3 3 0 1 0 0 6 3 3 0 0 0 0-6z'],
  'eye-off': ['M3 3l18 18', 'M10.6 6.1A9.7 9.7 0 0 1 12 6c6.4 0 10 6 10 6a17 17 0 0 1-3.1 3.9', 'M6.6 6.6A17 17 0 0 0 2 12s3.6 6 10 6a9.6 9.6 0 0 0 5.4-1.6', 'M9.9 9.9a3 3 0 0 0 4.2 4.2'],
  image: ['M4 4h16v16H4z', 'M4 16l4.5-4.5L13 16l2.5-2.5L20 18', 'M15 7.5a1.5 1.5 0 1 0 0 3 1.5 1.5 0 0 0 0-3z'],
  copy: ['M9 9h11v11H9z', 'M5 15H4V4h11v1'],
  download: ['M12 4v11M7 10l5 5 5-5M4 20h16'],
  home: ['M3 11l9-7 9 7v9a1 1 0 0 1-1 1h-5v-6H9v6H4a1 1 0 0 1-1-1v-9z'],
  book: ['M4 19.5A2.5 2.5 0 0 1 6.5 17H20V3H6.5A2.5 2.5 0 0 0 4 5.5v14z', 'M4 19.5A2.5 2.5 0 0 0 6.5 22H20v-5'],
  clipboard: ['M9 3h6v4H9z', 'M9 5H6a1 1 0 0 0-1 1v14a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1V6a1 1 0 0 0-1-1h-3', 'M9 12h6M9 16h4'],
  award: ['M12 3a6 6 0 1 0 0 12 6 6 0 0 0 0-12z', 'M8.5 13.5 7 22l5-3 5 3-1.5-8.5'],
  layout: ['M4 4h16v16H4z', 'M4 9h16M10 9v11'],
  text: ['M4 6h16M4 12h16M4 18h10'],
  users: ['M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2', 'M9 3a4 4 0 1 0 0 8 4 4 0 0 0 0-8z', 'M22 21v-2a4 4 0 0 0-3-3.9M16 3.1a4 4 0 0 1 0 7.8'],
  tag: ['M3 12V4h8l10 10-8 8L3 12z', 'M7.5 7a.5.5 0 1 0 0 1 .5.5 0 0 0 0-1z'],
  link: ['M10 14a4 4 0 0 0 5.7 0l3-3a4 4 0 0 0-5.7-5.7l-1 1', 'M14 10a4 4 0 0 0-5.7 0l-3 3a4 4 0 0 0 5.7 5.7l1-1'],
  logout: ['M9 21H5a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1h4', 'M16 17l5-5-5-5M21 12H9'],
  chart: ['M4 20V10M10 20V4M16 20v-7M2 20h20'],
  clock: ['M12 3a9 9 0 1 0 0 18 9 9 0 0 0 0-18z', 'M12 7v5l3 2'],
  shield: ['M12 3l8 3v6c0 5-3.5 8-8 9-4.5-1-8-4-8-9V6l8-3z', 'M9 12l2 2 4-4'],
  mail: ['M4 6h16v12H4z', 'M4 7l8 6 8-6'],
} as const;

export type IconName = keyof typeof ICONS;

/**
 * Satır içi SVG simge. Rengini yazı renginden (currentColor), boyutunu yazı boyutundan alır.
 * Süs amaçlıdır; anlamı olan butonlarda yazı ya da aria-label ayrıca verilmeli.
 */
@Component({
  selector: 'ygt-icon',
  template: `
    <svg
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      stroke-width="2"
      stroke-linecap="round"
      stroke-linejoin="round"
      aria-hidden="true"
      focusable="false"
    >
      @for (d of paths(); track $index) {
        <path [attr.d]="d" />
      }
    </svg>
  `,
  host: { class: 'icon' },
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Icon {
  readonly name = input.required<IconName>();

  protected readonly paths = computed(() => ICONS[this.name()]);
}
