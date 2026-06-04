import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../auth/services/auth.service';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

interface NavItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  currentUserRole = this.authService.userRole;
  userEmail = computed(() => this.authService.currentUserEmail() || 'Usuario');

  menuItems = computed<NavItem[]>(() => {
    const role = this.currentUserRole();
    const items: NavItem[] = [];

    if (!role) return items;

    if (role === 'Cliente') {
      items.push(
        { label: 'Ver Catálogo', icon: 'restaurant_menu', route: 'catalog' },
        { label: 'Mis Pedidos', icon: 'receipt_long', route: 'my-orders' }
      );
    }

    if (role === 'Preparador') {
      items.push(
        { label: 'Control de Cocina', icon: 'soup_kitchen', route: 'kitchen-orders' }
      );
    }

    if (role === 'Administrativo') {
      items.push(
        { label: 'Gestión de Pedidos', icon: 'local_shipping', route: 'admin-orders' },
        { label: 'Usuarios', icon: 'people', route: 'users' },
        { label: 'Productos', icon: 'bakery_dining', route: 'products' },
        { label: 'Promociones', icon: 'auto_awesome', route: 'promotions' },
        { label: 'Tipos de Envío', icon: 'currency_exchange', route: 'delivery-types' },
        { label: 'Importador Plugins', icon: 'extension', route: 'product-importer' },
        { label: 'Auditoría', icon: 'manage_search', route: 'audit-logs' },
        { label: 'Reportes y Estadísticas', icon: 'analytics', route: 'reports' }
      );
    }

    return items;
  });

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
