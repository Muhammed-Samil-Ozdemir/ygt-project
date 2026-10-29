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
import { HttpService, ToastService, UserCommand, UserListItem, UserUpdateCommand } from '@ygt/libraries';

/** Yeni yönetici (/users/new) ve ad/e-posta düzenleme (/users/:id/edit) aynı bileşen. */
@Component({
  imports: [FormsModule, RouterLink],
  templateUrl: './user-editor.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class UserEditor implements OnInit {
  readonly #http = inject(HttpService);
  readonly #router = inject(Router);
  readonly #toast = inject(ToastService);

  /** Düzenlemede adresten gelir, yeni yöneticide boş. */
  readonly id = input<string>();

  protected readonly loaded = signal(false);
  protected readonly saving = signal(false);
  protected model: UserCommand = { name: '', email: '', password: '' };
  protected passwordRepeat = '';

  ngOnInit() {
    const id = this.id();
    if (!id) {
      this.loaded.set(true);
      return;
    }

    this.#http.get<UserListItem[]>('/api/users', (users) => {
      const user = users.find((item) => item.id === id);
      if (!user) {
        this.#toast.error('Yönetici bulunamadı.');
        this.#router.navigateByUrl('/users');
        return;
      }

      this.model = { name: user.name, email: user.email, password: '' };
      this.loaded.set(true);
    });
  }

  protected save() {
    const id = this.id();
    if (!id && this.model.password !== this.passwordRepeat) {
      this.#toast.error('Şifreler aynı değil.');
      return;
    }

    const done = () => {
      this.#toast.success(id ? 'Yönetici güncellendi.' : 'Yönetici eklendi.');
      this.#router.navigateByUrl('/users');
    };
    const failed = () => this.saving.set(false);

    this.saving.set(true);
    if (id) {
      const command: UserUpdateCommand = { name: this.model.name, email: this.model.email };
      this.#http.put(`/api/users/${id}`, command, done, failed);
    } else {
      this.#http.post('/api/users', this.model, done, failed);
    }
  }
}
