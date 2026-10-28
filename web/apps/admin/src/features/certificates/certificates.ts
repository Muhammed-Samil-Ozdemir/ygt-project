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
import { CertificateListItem, HttpService, ToastService } from '@ygt/libraries';

@Component({
  imports: [DatePipe, RouterLink],
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

  ngOnInit() {
    this.#load();
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
