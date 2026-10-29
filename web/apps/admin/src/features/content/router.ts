import { Routes } from '@angular/router';

const router: Routes = [
  {
    path: '',
    loadComponent: () => import('./content'),
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'texts',
      },
      {
        path: 'texts',
        loadComponent: () => import('./texts/texts'),
      },
      {
        path: 'about',
        loadComponent: () => import('./about/about'),
      },
      {
        path: 'team',
        loadComponent: () => import('./team/team'),
      },
      {
        path: 'technologies',
        loadComponent: () => import('./technologies/technologies'),
      },
      {
        path: 'links',
        loadComponent: () => import('./links/links'),
      },
    ],
  },
];

export default router;
