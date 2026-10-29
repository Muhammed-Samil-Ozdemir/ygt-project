import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChangePasswordCommand, CurrentUser, HttpService, MessageResponse, ToastService } from '@ygt/libraries';

/** Giriş yapmış yöneticinin bilgileri ve kendi şifresini değiştirmesi. */
@Component({
  imports: [FormsModule],
  templateUrl: './account.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Account implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  protected readonly user = signal<CurrentUser | null>(null);
  protected readonly saving = signal(false);
  protected model: ChangePasswordCommand = { currentPassword: '', newPassword: '' };
  protected passwordRepeat = '';

  ngOnInit() {
    this.#http.get<CurrentUser>('/api/auth/me', (user) => this.user.set(user));
  }

  protected changePassword() {
    if (this.model.newPassword !== this.passwordRepeat) {
      this.#toast.error('Yeni şifreler aynı değil.');
      return;
    }

    this.saving.set(true);
    this.#http.put<MessageResponse>(
      '/api/auth/password',
      this.model,
      (res) => {
        this.saving.set(false);
        this.model = { currentPassword: '', newPassword: '' };
        this.passwordRepeat = '';
        this.#toast.success(res.message);
      },
      () => this.saving.set(false),
    );
  }
}
