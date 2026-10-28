import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastContainer } from '@ygt/libraries';
import { SiteService } from './services/site';

@Component({
  imports: [RouterOutlet, ToastContainer],
  selector: 'app-root',
  template: '<router-outlet /><ygt-toasts />',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {
  readonly #site = inject(SiteService);

  constructor() {
    this.#site.load();
    this.#site.recordVisit();
  }
}
