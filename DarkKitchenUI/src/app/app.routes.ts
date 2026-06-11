import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/components/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/components/register/register.component').then(
        (m) => m.RegisterComponent,
      ),
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/components/dashboard/dashboard.component').then(
        (m) => m.DashboardComponent,
      ),
    children: [
      {
        path: 'products',
        canActivate: [roleGuard(['Administrativo'])],
        loadComponent: () =>
          import('./features/products/components/product-list/product-list.component').then(
            (m) => m.ProductListComponent,
          ),
      },
      {
        path: 'admin-orders',
        canActivate: [roleGuard(['Administrativo'])],
        loadComponent: () =>
          import('./features/orders/components/admin-orders/admin-orders.component').then(
            (m) => m.AdminOrdersComponent,
          ),
      },
      {
        path: 'audit-logs',
        canActivate: [roleGuard(['Administrativo'])],
        loadComponent: () =>
          import('./features/audit-logs/components/audit-log-list/audit-log-list.component').then(
            (m) => m.AuditLogListComponent,
          ),
      },
    ],
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
];
