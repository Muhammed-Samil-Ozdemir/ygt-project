import { ChangeDetectionStrategy, Component, inject, signal, ViewEncapsulation } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Icon, IconName, tokenStore } from '@ygt/libraries';
import { BrandService } from '../services/brand';

@Component({
  imports: [Icon, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './layout.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Layout {
  readonly #router = inject(Router);

  protected readonly brand = inject(BrandService);

  protected readonly nav: { link: string; label: string; icon: IconName; exact: boolean }[] = [
    { link: '/', label: 'Panel', icon: 'home', exact: true },
    { link: '/courses', label: 'Eğitimler', icon: 'book', exact: false },
    { link: '/forms', label: 'Formlar', icon: 'clipboard', exact: false },
    { link: '/certificates', label: 'Sertifikalar', icon: 'award', exact: false },
    { link: '/content', label: 'Site içeriği', icon: 'layout', exact: false },
    { link: '/users', label: 'Yöneticiler', icon: 'users', exact: false },
  ];

  /** Dar ekranda menü üstte kapalı durur, butonla açılır. */
  protected readonly menuOpen = signal(false);

  protected closeMenu() {
    this.menuOpen.set(false);
  }

  /** Token süreli olduğu için sunucuda ayrıca çıkış işlemi yok; token silinince oturum kapanır. */
  protected logout() {
    tokenStore.clear();
    this.#router.navigateByUrl('/login');
  }
}
