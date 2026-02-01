import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    title: 'SAGA Dashboard'
  },
  {
    path: 'create',
    loadComponent: () => import('./features/create-trip/create-trip.component').then(m => m.CreateTripComponent),
    title: 'Create Trip'
  },
  {
    path: 'trip/:id',
    loadComponent: () => import('./features/trip-details/trip-details.component').then(m => m.TripDetailsComponent),
    title: 'Trip Details'
  },
  {
    path: '**',
    redirectTo: ''
  }
];
