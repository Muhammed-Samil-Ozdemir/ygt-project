import { ChangeDetectionStrategy, Component, inject, ViewEncapsulation } from '@angular/core';
import { ToastService } from '../services/toast';

@Component({
  selector: 'ygt-toasts',
  template: `
    <div class="toasts" aria-live="polite">
      @for (toast of toasts.items(); track toast.id) {
        <div class="toast toast-{{ toast.type }}" role="status" (click)="toasts.dismiss(toast.id)">
          {{ toast.message }}
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
