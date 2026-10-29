/** Sekme simgesini değiştirir. Sayfada simge bağlantısı yoksa ekler. */
export function setFavicon(document: Document, url: string) {
  let link = document.querySelector<HTMLLinkElement>('link[rel="icon"]');
  if (!link) {
    link = document.createElement('link');
    link.rel = 'icon';
    document.head.appendChild(link);
  }
  link.href = url;
}
