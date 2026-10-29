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
import { SiteService } from '../../services/site';

@Component({
  imports: [Icon, RouterLink],
  templateUrl: './certificate.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CertificatePage implements OnInit {
  readonly #http = inject(HttpService);
  readonly #site = inject(SiteService);

  /** Adresteki doğrulama kodu (/certificates/:code). */
  readonly code = input.required<string>();

  protected readonly certificate = signal<Certificate | null>(null);
  protected readonly notFound = signal(false);
  protected readonly imageUrl = computed(() => `/api/certificates/${encodeURIComponent(this.code())}/image`);
  /** Doğrulama kodunun önündeki not ("Bu sertifika ... tarafından verilmiştir."); panelden gelir. */
  protected readonly note = computed(() => this.#site.content()?.texts.certificateNote ?? '');

  ngOnInit() {
    this.#http.get<Certificate>(
      `/api/certificates/${encodeURIComponent(this.code())}`,
      (certificate) => this.certificate.set(certificate),
      () => this.notFound.set(true),
      { quiet: true },
    );
  }
}
