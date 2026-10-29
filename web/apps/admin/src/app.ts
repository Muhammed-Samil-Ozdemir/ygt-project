import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastContainer } from '@ygt/libraries';
import { BrandService } from './services/brand';

@Component({
  imports: [RouterOutlet, ToastContainer],
  selector: 'app-root',
  template: '<router-outlet /><ygt-toasts />',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {
  readonly #brand = inject(BrandService);

  constructor() {
    this.#brand.load();
  }
}
