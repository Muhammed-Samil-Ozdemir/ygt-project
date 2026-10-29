import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { RouterLink, RouterOutlet } from '@angular/router';
import { Icon, siteImageUrl } from '@ygt/libraries';
import { AccentText } from '../components/accent-text';
import { SiteService } from '../services/site';

@Component({
  imports: [AccentText, Icon, NgTemplateOutlet, RouterLink, RouterOutlet],
  templateUrl: './layout.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '(window:scroll)': 'onScroll()' },
})
export default class Layout {
  readonly #site = inject(SiteService);

  protected readonly content = this.#site.content;
  protected readonly imageUrl = siteImageUrl;
  protected readonly year = new Date().getFullYear();
  protected readonly menuOpen = signal(false);
  /** Sayfa kaydırılınca üst menünün altına gölge düşer. */
  protected readonly scrolled = signal(false);

  /** Admin panelindeki ayara göre üst menüde eğitim ya da etkinlik başvurusu gösterilir. Yazısı da panelden gelir. */
  protected readonly application = computed(() => {
    const texts = this.#site.content()?.texts;
    if (!texts) {
      return null;
    }

    switch (this.#site.settings()?.homeButton) {
      case 'course':
        return { label: texts.courseButton, link: '/appeal' };
      case 'form':
        return { label: texts.formButton, link: '/form' };
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
