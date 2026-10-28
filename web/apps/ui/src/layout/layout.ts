import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { LINKS } from '../constants';
import { SiteService } from '../services/site';

@Component({
  imports: [RouterLink, RouterOutlet],
  templateUrl: './layout.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Layout {
  readonly #site = inject(SiteService);

  protected readonly links = LINKS;
  protected readonly year = new Date().getFullYear();
  protected readonly menuOpen = signal(false);

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
}
