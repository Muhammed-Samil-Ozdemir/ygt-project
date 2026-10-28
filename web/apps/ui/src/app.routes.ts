import { Route } from '@angular/router';

// Eski sitedeki adresler (/appeal, /form, /hackathon, /counter) paylaşılmış linkler bozulmasın diye korunuyor.
export const appRoutes: Route[] = [
  {
    path: 'counter',
    loadComponent: () => import('./features/counter/counter'),
  },
  {
    // Eski sistemdeki sertifika linki: /certs?certID=...
    path: 'certs',
    redirectTo: ({ queryParams }) => `/certificates/${queryParams['certID'] ?? ''}`,
  },
  {
    path: '',
    loadComponent: () => import('./layout/layout'),
    children: [
      {
        path: '',
        loadComponent: () => import('./features/home/home'),
      },
      {
        path: 'home',
        redirectTo: '',
        pathMatch: 'full',
      },
      {
        path: 'appeal',
        loadComponent: () => import('./features/course-application/course-application'),
      },
      {
        path: 'form',
        loadComponent: () => import('./features/form-application/form-application'),
      },
      {
        path: 'hackathon',
        loadComponent: () => import('./features/hackathon-application/hackathon-application'),
      },
      {
        path: 'certificates/:code',
        loadComponent: () => import('./features/certificate/certificate'),
      },
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];
