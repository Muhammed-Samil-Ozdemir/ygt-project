import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  input,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { Certificate, HttpService, Icon } from '@ygt/libraries';

@Component({
  imports: [Icon, RouterLink],
  templateUrl: './certificate.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CertificatePage implements OnInit {
  readonly #http = inject(HttpService);

  /** Adresteki doğrulama kodu (/certificates/:code). */
  readonly code = input.required<string>();

  protected readonly certificate = signal<Certificate | null>(null);
  protected readonly notFound = signal(false);
  protected readonly imageUrl = computed(() => `/api/certificates/${encodeURIComponent(this.code())}/image`);

  ngOnInit() {
    this.#http.get<Certificate>(
      `/api/certificates/${encodeURIComponent(this.code())}`,
      (certificate) => this.certificate.set(certificate),
      () => this.notFound.set(true),
      { quiet: true },
    );
  }
}
