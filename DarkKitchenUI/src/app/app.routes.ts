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
        path: 'kitchen-orders',
        canActivate: [roleGuard(['Preparador'])],
        loadComponent: () =>
          import('./features/orders/components/kitchen-orders/kitchen-orders.component').then(
            (m) => m.KitchenOrdersComponent,
          ),
      },
      {
        path: 'my-orders',
        canActivate: [roleGuard(['Cliente'])],
        loadComponent: () =>
          import('./features/orders/components/my-orders/my-orders.component').then(
            (m) => m.MyOrdersComponent,
          ),
      },
      {
        path: 'catalog',
        canActivate: [roleGuard(['Cliente'])],
        loadComponent: () =>
          import('./features/catalog/components/client-catalog/catalog.component').then(
            (m) => m.CatalogComponent,
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
      {
        path: 'users',
        canActivate: [roleGuard(['Administrativo'])],
        loadComponent: () =>
          import('./features/users/components/user-list/user-list.component').then(
            (m) => m.UserListComponent,
          ),
      },
      {
        path: 'product-importer',
        canActivate: [roleGuard(['Administrativo'])],
        loadComponent: () =>
          import('./features/product-importer/components/product-importer/product-importer.component').then(
            (m) => m.ProductImporterComponent,
          ),
      },
      {
        path: 'reports',
        canActivate: [roleGuard(['Administrativo'])],
        loadComponent: () =>
          import('./features/reports/components/reports-dashboard/reports-dashboard.component').then(
            (m) => m.ReportsDashboardComponent,
          ),
      },
      {
        path: 'promotions',
        canActivate: [roleGuard(['Administrativo'])],
        loadComponent: () =>
          import('./features/promotions/components/promotion-list/promotion-list.component').then(
            (m) => m.PromotionListComponent,
          ),
      },
      {
        path: 'delivery-types',
        canActivate: [roleGuard(['Administrativo'])],
        loadComponent: () =>
          import('./features/shipping/components/shipping-type-list/shipping-type-list.component').then(
            (m) => m.ShippingTypeListComponent,
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
