import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { Icon } from '@ygt/libraries';
import { AccentText } from '../components/accent-text';
import { SiteService } from '../services/site';

@Component({
  imports: [AccentText, Icon, RouterLink, RouterOutlet],
  templateUrl: './layout.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '(window:scroll)': 'onScroll()' },
})
export default class Layout {
  readonly #site = inject(SiteService);

  protected readonly content = this.#site.content;
  protected readonly year = new Date().getFullYear();
  protected readonly menuOpen = signal(false);
  /** Sayfa kaydırılınca üst menünün altına gölge düşer. */
  protected readonly scrolled = signal(false);

  /** Admin panelindeki ayara göre üst menüde eğitim ya da etkinlik başvurusu gösterilir. */
  protected readonly application = computed(() => {
    switch (this.#site.settings()?.homeButton) {
      case 'course':
        return { label: 'Eğitim Başvurusu', link: '/appeal' };
      case 'form':
        return { label: 'Etkinlik Başvurusu', link: '/form' };
      default:
        return null;
    }
  });

  protected closeMenu() {
    this.menuOpen.set(false);
  }

  protected onScroll() {
    this.scrolled.set(window.scrollY > 8);
  }
}
