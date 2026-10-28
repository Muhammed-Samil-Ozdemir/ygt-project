import { Routes } from '@angular/router';

const router: Routes = [
  {
    path: '',
    loadComponent: () => import('./forms'),
  },
  {
    path: 'new',
    loadComponent: () => import('./form-editor/form-editor'),
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./form-editor/form-editor'),
  },
  {
    path: ':id/applications',
    loadComponent: () => import('./form-applications/form-applications'),
  },
];

export default router;
