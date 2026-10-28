import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'info';

export interface Toast {
  id: number;
  type: ToastType;
  message: string;
}

/** Ekranın köşesinde birkaç saniye görünen bildirimler. */
@Injectable({
  providedIn: 'root',
})
export class ToastService {
  readonly #items = signal<Toast[]>([]);
  readonly items = this.#items.asReadonly();
  #nextId = 0;

  success(message: string) {
    this.#show('success', message);
  }

  error(message: string) {
    this.#show('error', message);
  }

  info(message: string) {
    this.#show('info', message);
  }

  dismiss(id: number) {
    this.#items.update((items) => items.filter((toast) => toast.id !== id));
  }

  #show(type: ToastType, message: string) {
    const id = ++this.#nextId;
    this.#items.update((items) => [...items, { id, type, message }]);
    setTimeout(() => this.dismiss(id), 5000);
  }
}
