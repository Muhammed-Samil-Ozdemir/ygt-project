import { inject, Injectable, signal } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';
import { HttpService, SiteContent, SiteSettings } from '@ygt/libraries';

/**
 * Site ayarları (ana sayfa butonu, geri sayım) ve panelden yönetilen içerik (metinler, ekip, bağlantılar...).
 * Uygulama açılırken bir kez yüklenir; panelde yapılan değişiklik site yenilenince görünür.
 */
@Injectable({
  providedIn: 'root',
})
export class SiteService {
  readonly #http = inject(HttpService);
  readonly #title = inject(Title);
  readonly #meta = inject(Meta);
  readonly #settings = signal<SiteSettings | null>(null);
  readonly #content = signal<SiteContent | null>(null);

  readonly settings = this.#settings.asReadonly();
  /** Yüklenene kadar null. */
  readonly content = this.#content.asReadonly();

  load() {
    this.#http.get<SiteSettings>('/api/site/settings', (res) => this.#settings.set(res), undefined, { quiet: true });
    this.#http.get<SiteContent>(
      '/api/site/content',
      (content) => {
        this.#content.set(content);
        // Sayfa başlığı ve açıklaması da panelden gelir; index.html'dekiler içerik yüklenene kadar görünür.
        this.#title.setTitle(content.texts.seoTitle);
        this.#meta.updateTag({ name: 'description', content: content.texts.seoDescription });
      },
      undefined,
      { quiet: true },
    );
  }

  /** Ana sayfa ziyaret sayacını artırır. Hata olursa kullanıcıya gösterilmez. */
  recordVisit() {
    this.#http.post('/api/site/visits/home', null, () => undefined, undefined, { quiet: true });
  }
}
