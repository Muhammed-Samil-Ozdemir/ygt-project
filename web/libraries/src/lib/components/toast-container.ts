import { ChangeDetectionStrategy, Component, inject, ViewEncapsulation } from '@angular/core';
import { ToastService } from '../services/toast';
import { Icon } from './icon';

@Component({
  selector: 'ygt-toasts',
  imports: [Icon],
  template: `
    <div class="toasts" aria-live="polite">
      @for (toast of toasts.items(); track toast.id) {
        <div class="toast toast-{{ toast.type }}" role="status" (click)="toasts.dismiss(toast.id)">
          <span class="toast-icon">
            <ygt-icon [name]="toast.type === 'success' ? 'check' : toast.type === 'error' ? 'x' : 'chevron-right'" />
          </span>
          <span>{{ toast.message }}</span>
        </div>
      }
    </div>
  `,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToastContainer {
  protected readonly toasts = inject(ToastService);
}
