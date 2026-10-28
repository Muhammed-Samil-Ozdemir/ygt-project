import { ChangeDetectionStrategy, Component, inject, ViewEncapsulation } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { tokenStore } from '@ygt/libraries';

@Component({
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './layout.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Layout {
  readonly #router = inject(Router);

  /** Token süreli olduğu için sunucuda ayrıca çıkış işlemi yok; token silinince oturum kapanır. */
  protected logout() {
    tokenStore.clear();
    this.#router.navigateByUrl('/login');
  }
}
