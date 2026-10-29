import { Route } from '@angular/router';
import { authGuard } from './guards/auth-guard';

export const appRoutes: Route[] = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login'),
  },
  {
    path: '',
    loadComponent: () => import('./layout/layout'),
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () => import('./features/home/home'),
      },
      {
        path: 'courses',
        loadChildren: () => import('./features/courses/router'),
      },
      {
        path: 'forms',
        loadChildren: () => import('./features/forms/router'),
      },
      {
        path: 'certificates',
        loadChildren: () => import('./features/certificates/router'),
      },
      {
        path: 'content',
        loadChildren: () => import('./features/content/router'),
      },
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];
