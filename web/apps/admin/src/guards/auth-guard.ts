import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { tokenStore } from '@ygt/libraries';
import { jwtDecode } from 'jwt-decode';

/** Token var ve süresi dolmamışsa true. */
export function hasValidToken(): boolean {
  const token = tokenStore.get();
  if (!token) {
    return false;
  }

  try {
    const { exp } = jwtDecode(token);
    return (exp ?? 0) > Date.now() / 1000;
  } catch {
    return false;
  }
}

export const authGuard = () => {
  if (hasValidToken()) {
    return true;
  }

  tokenStore.clear();
  return inject(Router).createUrlTree(['/login']);
};
