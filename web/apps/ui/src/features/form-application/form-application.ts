import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  errorMessage,
  Form,
  FormApplicationCommand,
  HttpService,
} from '@ygt/libraries';

@Component({
  imports: [FormsModule, RouterLink],
  templateUrl: './form-application.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class FormApplication implements OnInit {
  readonly #http = inject(HttpService);
  readonly #router = inject(Router);

  protected readonly form = signal<Form | null>(null);
  /** Aktif form yoksa API'nin "başvurular sona erdi" mesajı. */
  protected readonly closedMessage = signal<string | null>(null);
  protected readonly sending = signal(false);
  protected readonly sent = signal(false);

  protected model: FormApplicationCommand = { firstName: '', lastName: '', email: '', phoneNumber: '' };

  ngOnInit() {
    this.#http.get<Form>(
      '/api/forms/active',
      (form) => {
        // Hackathon formu takım başvurusu alır, kendi sayfası var.
        if (form.type === 'hackathon') {
          this.#router.navigateByUrl('/hackathon', { replaceUrl: true });
          return;
        }
        this.form.set(form);
      },
      (err) => this.closedMessage.set(errorMessage(err)),
      { quiet: true },
    );
  }

  protected submit() {
    const form = this.form();
    if (!form) {
      return;
    }

    this.sending.set(true);
    this.#http.post(
      `/api/forms/${form.id}/applications`,
      this.model,
      () => {
        this.sending.set(false);
        this.sent.set(true);
      },
      () => this.sending.set(false),
    );
  }
}
