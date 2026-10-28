import { inject, Injectable, signal } from '@angular/core';
import { HttpService, SiteSettings } from '@ygt/libraries';

/** Site ayarları (ana sayfa butonu, geri sayım). Uygulama açılırken bir kez yüklenir. */
@Injectable({
  providedIn: 'root',
})
export class SiteService {
  readonly #http = inject(HttpService);
  readonly #settings = signal<SiteSettings | null>(null);

  readonly settings = this.#settings.asReadonly();

  load() {
    this.#http.get<SiteSettings>('/api/site/settings', (res) => this.#settings.set(res), undefined, { quiet: true });
  }

  /** Ana sayfa ziyaret sayacını artırır. Hata olursa kullanıcıya gösterilmez. */
  recordVisit() {
    this.#http.post('/api/site/visits/home', null, () => undefined, undefined, { quiet: true });
  }
}
