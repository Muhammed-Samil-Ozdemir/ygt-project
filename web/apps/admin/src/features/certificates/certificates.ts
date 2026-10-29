import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CertificateListItem, HttpService, Icon, MessageResponse, ToastService } from '@ygt/libraries';

@Component({
  imports: [DatePipe, Icon, RouterLink],
  templateUrl: './certificates.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Certificates implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  protected readonly certificates = signal<CertificateListItem[] | null>(null);
  /** Görseli yüklenmekte olan sertifikanın ID'si. */
  protected readonly uploadingId = signal<string | null>(null);
  protected readonly importing = signal(false);
  protected readonly exporting = signal(false);

  ngOnInit() {
    this.#load();
  }

  protected downloadTemplate() {
    this.#http.download('/api/certificates/import/template');
  }

  /** Excel'deki satırları toplu ekler. Hatalı satır varsa API hiçbirini eklemez ve satır numaralarını söyler. */
  protected importFile(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) {
      return;
    }

    this.importing.set(true);
    this.#http.upload<MessageResponse>(
      '/api/certificates/import',
      file,
      (res) => {
        this.importing.set(false);
        this.#toast.success(res.message);
        this.#load();
      },
      () => this.importing.set(false),
      'post',
    );
  }

  /** Bütün sertifikaları doğrulama linkleriyle Excel'e aktarır; linkler bu panelin adresiyle başlar. */
  protected exportLinks() {
    this.exporting.set(true);
    this.#http.download(
      `/api/certificates/export?siteUrl=${encodeURIComponent(location.origin)}`,
      () => this.exporting.set(false),
      () => this.exporting.set(false),
    );
  }

  /** Doğrulama linkinin QR kodunu PNG olarak indirir. Kütüphane sadece gerektiğinde yüklenir. */
  protected async downloadQr(certificate: CertificateListItem) {
    // qrcode CommonJS paketi; derlenmiş kodda fonksiyonları "default" altında gelir.
    const qrcode = await import('qrcode');
    const { toDataURL } = (qrcode as unknown as { default?: typeof qrcode }).default ?? qrcode;
    const dataUrl = await toDataURL(this.verifyUrl(certificate), { width: 600, margin: 2 });

    const link = document.createElement('a');
    link.href = dataUrl;
    link.download = `qr-${certificate.firstName}-${certificate.lastName}.png`.replace(/\s+/g, '-');
    link.click();
  }

  /** Sertifikanın üzerine basılacak / QR koda çevrilecek doğrulama linki. */
  protected verifyUrl(certificate: CertificateListItem) {
    return `${location.origin}/certificates/${certificate.code}`;
  }

  protected copyLink(certificate: CertificateListItem) {
    const url = this.verifyUrl(certificate);
    navigator.clipboard.writeText(url).then(
      () => this.#toast.success('Doğrulama linki kopyalandı.'),
      () => this.#toast.info(`Doğrulama linki: ${url}`),
    );
  }

  protected upload(certificate: CertificateListItem, event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) {
      return;
    }

    this.uploadingId.set(certificate.id);
    this.#http.upload(
      `/api/certificates/${certificate.id}/image`,
      file,
      () => {
        this.uploadingId.set(null);
        this.#toast.success('Görsel yüklendi.');
        this.#load();
      },
      () => this.uploadingId.set(null),
    );
  }

  protected remove(certificate: CertificateListItem) {
    if (!confirm(`${certificate.firstName} ${certificate.lastName} adına verilen sertifika silinsin mi?`)) {
      return;
    }

    this.#http.delete(`/api/certificates/${certificate.id}`, () => {
      this.#toast.success('Sertifika silindi.');
      this.#load();
    });
  }

  #load() {
    this.#http.get<CertificateListItem[]>('/api/certificates', (certificates) => this.certificates.set(certificates));
  }
}
