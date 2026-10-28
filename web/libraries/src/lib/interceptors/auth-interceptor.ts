import { HttpInterceptorFn } from '@angular/common/http';
import { tokenStore } from '../services/token';

/** Token varsa isteğe "Authorization: Bearer ..." başlığını ekler. */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = tokenStore.get();

  return next(token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req);
};
