import { Routes } from '@angular/router';

const router: Routes = [
  {
    path: '',
    loadComponent: () => import('./certificates'),
  },
  {
    path: 'new',
    loadComponent: () => import('./certificate-editor/certificate-editor'),
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./certificate-editor/certificate-editor'),
  },
];

export default router;
