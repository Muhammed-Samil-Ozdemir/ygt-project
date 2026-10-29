import { DOCUMENT, inject, Injectable, signal } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';
import { HttpService, setFavicon, SiteContent, SiteSettings, siteImageUrl, SiteTexts } from '@ygt/libraries';

/**
 * Site ayarları (ana sayfa butonu, geri sayım) ve panelden yönetilen içerik (metinler, görseller, ekip...).
 * Uygulama açılırken bir kez yüklenir; panelde yapılan değişiklik site yenilenince görünür.
 */
@Injectable({
  providedIn: 'root',
})
export class SiteService {
  readonly #http = inject(HttpService);
  readonly #title = inject(Title);
  readonly #meta = inject(Meta);
  readonly #document = inject(DOCUMENT);
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
        this.#applyHead(content.texts);
      },
      undefined,
      { quiet: true },
    );
  }

  /**
   * Sekme başlığı, açıklama ve sekme simgesi de panelden gelir. index.html'deki başlık ve açıklama içerik
   * yüklenene kadar (ve panelde boş bırakılırsa) görünür.
   */
  #applyHead(texts: SiteTexts) {
    const title = texts.seoTitle || texts.siteName;
    if (title) {
      this.#title.setTitle(title);
    }
    if (texts.seoDescription) {
      this.#meta.updateTag({ name: 'description', content: texts.seoDescription });
    }
    const icon = texts.favicon || texts.logo;
    if (icon) {
      setFavicon(this.#document, siteImageUrl(icon));
    }
  }

  /** Ana sayfa ziyaret sayacını artırır. Hata olursa kullanıcıya gösterilmez. */
  recordVisit() {
    this.#http.post('/api/site/visits/home', null, () => undefined, undefined, { quiet: true });
  }
}
