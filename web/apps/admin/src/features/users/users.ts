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
import { HttpService, Icon, ToastService, UserListItem } from '@ygt/libraries';

@Component({
  imports: [DatePipe, Icon, RouterLink],
  templateUrl: './users.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Users implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  protected readonly users = signal<UserListItem[] | null>(null);
  /** Silinmekte olan yöneticinin ID'si. */
  protected readonly deletingId = signal<string | null>(null);

  ngOnInit() {
    this.#http.get<UserListItem[]>('/api/users', (users) => this.users.set(users));
  }

  protected remove(user: UserListItem) {
    if (!confirm(`${user.name} yöneticiler arasından silinsin mi? Panele bir daha giriş yapamaz.`)) {
      return;
    }

    this.deletingId.set(user.id);
    this.#http.delete(
      `/api/users/${user.id}`,
      () => {
        this.users.update((list) => list?.filter((item) => item.id !== user.id) ?? null);
        this.deletingId.set(null);
        this.#toast.success('Yönetici silindi.');
      },
      () => this.deletingId.set(null),
    );
  }
}
