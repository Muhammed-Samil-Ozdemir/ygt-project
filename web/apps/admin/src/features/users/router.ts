import { Routes } from '@angular/router';

const router: Routes = [
  {
    path: '',
    loadComponent: () => import('./users'),
  },
  {
    path: 'new',
    loadComponent: () => import('./user-editor/user-editor'),
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./user-editor/user-editor'),
  },
  {
    path: ':id/password',
    loadComponent: () => import('./user-password/user-password'),
  },
];

export default router;
