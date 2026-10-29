import { DOCUMENT, inject, Injectable, signal } from '@angular/core';
import { HttpService, setFavicon, SiteContent, siteImageUrl } from '@ygt/libraries';

/**
 * Panelde görünen logo ve topluluk adı: sitenin "Site içeriği > Metinler ve görseller" bölümünden girilenler.
 * Panel açılırken yüklenir; logo, sekme simgesi ya da ad değişince yeniden yüklenir.
 */
@Injectable({
  providedIn: 'root',
})
export class BrandService {
  readonly #http = inject(HttpService);
  readonly #document = inject(DOCUMENT);
  readonly #logo = signal<string | null>(null);
  readonly #siteName = signal('');

  /** Logonun adresi; logo yüklenmemişse null. */
  readonly logo = this.#logo.asReadonly();
  /** Girilmemişse "". */
  readonly siteName = this.#siteName.asReadonly();

  load() {
    this.#http.get<SiteContent>(
      '/api/site/content',
      ({ texts }) => {
        this.#logo.set(texts.logo ? siteImageUrl(texts.logo) : null);
        this.#siteName.set(texts.siteName);

        const icon = texts.favicon || texts.logo;
        if (icon) {
          setFavicon(this.#document, siteImageUrl(icon));
        }
      },
      undefined,
      { quiet: true },
    );
  }
}
