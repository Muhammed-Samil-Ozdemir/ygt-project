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
import { HttpService, ToastService, UserListItem } from '@ygt/libraries';

/** Başka bir yöneticiye yeni şifre belirler (ör. şifresini unuttuysa). Kendi şifresi /account'tan değişir. */
@Component({
  imports: [FormsModule, RouterLink],
  templateUrl: './user-password.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class UserPassword implements OnInit {
  readonly #http = inject(HttpService);
  readonly #router = inject(Router);
  readonly #toast = inject(ToastService);

  readonly id = input.required<string>();

  protected readonly user = signal<UserListItem | null>(null);
  protected readonly saving = signal(false);
  protected password = '';
  protected passwordRepeat = '';

  ngOnInit() {
    this.#http.get<UserListItem[]>('/api/users', (users) => {
      const user = users.find((item) => item.id === this.id());
      if (!user || user.isCurrent) {
        this.#router.navigateByUrl(user?.isCurrent ? '/account' : '/users');
        return;
      }
      this.user.set(user);
    });
  }

  protected save() {
    if (this.password !== this.passwordRepeat) {
      this.#toast.error('Şifreler aynı değil.');
      return;
    }

    this.saving.set(true);
    this.#http.put(
      `/api/users/${this.id()}/password`,
      { password: this.password },
      () => {
        this.#toast.success('Yeni şifre belirlendi.');
        this.#router.navigateByUrl('/users');
      },
      () => this.saving.set(false),
    );
  }
}
