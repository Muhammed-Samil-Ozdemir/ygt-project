import {
  ChangeDetectionStrategy,
  Component,
  inject,
  input,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CertificateCommand, CertificateListItem, HttpService, ToastService } from '@ygt/libraries';

/** Yeni sertifika (/certificates/new) ve düzenleme (/certificates/:id/edit) aynı bileşen. */
@Component({
  imports: [FormsModule, RouterLink],
  templateUrl: './certificate-editor.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CertificateEditor implements OnInit {
  readonly #http = inject(HttpService);
  readonly #router = inject(Router);
  readonly #toast = inject(ToastService);

  readonly id = input<string>();

  protected readonly loaded = signal(false);
  protected readonly saving = signal(false);
  protected model: CertificateCommand = { courseName: '', firstName: '', lastName: '', email: '', phoneNumber: '' };

  ngOnInit() {
    const id = this.id();
    if (!id) {
      this.loaded.set(true);
      return;
    }

    this.#http.get<CertificateListItem[]>('/api/certificates', (certificates) => {
      const certificate = certificates.find((item) => item.id === id);
      if (!certificate) {
        this.#toast.error('Sertifika bulunamadı.');
        this.#router.navigateByUrl('/certificates');
        return;
      }

      this.model = {
        courseName: certificate.courseName,
        firstName: certificate.firstName,
        lastName: certificate.lastName,
        email: certificate.email ?? '',
        phoneNumber: certificate.phoneNumber ?? '',
      };
      this.loaded.set(true);
    });
  }

  protected save() {
    const id = this.id();
    const done = () => {
      this.#toast.success(id ? 'Sertifika güncellendi.' : 'Sertifika oluşturuldu. Görseli listeden yükleyebilirsiniz.');
      this.#router.navigateByUrl('/certificates');
    };
    const failed = () => this.saving.set(false);

    this.saving.set(true);
    if (id) {
      this.#http.put(`/api/certificates/${id}`, this.model, done, failed);
    } else {
      this.#http.post('/api/certificates', this.model, done, failed);
    }
  }
}
