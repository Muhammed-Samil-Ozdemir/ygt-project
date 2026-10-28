import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpService, LoginRequest, TokenResponse, tokenStore } from '@ygt/libraries';
import { hasValidToken } from '../../../guards/auth-guard';

@Component({
  imports: [FormsModule],
  templateUrl: './login.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Login implements OnInit {
  readonly #http = inject(HttpService);
  readonly #router = inject(Router);

  protected readonly sending = signal(false);
  protected model: LoginRequest = { email: '', password: '' };

  ngOnInit() {
    // Zaten giriş yapılmışsa panele geç.
    if (hasValidToken()) {
      this.#router.navigateByUrl('/');
    }
  }

  protected login() {
    this.sending.set(true);
    this.#http.post<TokenResponse>(
      '/api/auth/login',
      this.model,
      (res) => {
        tokenStore.set(res.token);
        this.#router.navigateByUrl('/');
      },
      () => this.sending.set(false),
    );
  }
}
