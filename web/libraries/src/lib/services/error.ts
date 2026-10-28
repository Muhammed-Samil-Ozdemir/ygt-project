import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ToastService } from './toast';
import { tokenStore } from './token';

/** API her hatada { message } döndürür; mesaj yoksa genel bir metin gösterilir. */
export function errorMessage(err: HttpErrorResponse): string {
  if (err.status === 0) {
    return 'Sunucuya ulaşılamadı. İnternet bağlantınızı kontrol edip tekrar deneyin.';
  }

  return typeof err.error?.message === 'string'
    ? err.error.message
    : 'Beklenmeyen bir hata oluştu. Lütfen tekrar deneyin.';
}

@Injectable({
  providedIn: 'root',
})
export class ErrorService {
  readonly #router = inject(Router);
  readonly #toast = inject(ToastService);

  handle(err: HttpErrorResponse) {
    // Admin oturumu düşmüşse token silinip giriş sayfasına dönülür.
    if (err.status === 401 && tokenStore.get()) {
      tokenStore.clear();
      this.#router.navigateByUrl('/login');
    }

    this.#toast.error(errorMessage(err));
  }
}
