import { Routes } from '@angular/router';

const router: Routes = [
  {
    path: '',
    loadComponent: () => import('./courses'),
  },
  {
    path: 'new',
    loadComponent: () => import('./course-editor/course-editor'),
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./course-editor/course-editor'),
  },
  {
    path: ':id/applications',
    loadComponent: () => import('./course-applications/course-applications'),
  },
];

export default router;
