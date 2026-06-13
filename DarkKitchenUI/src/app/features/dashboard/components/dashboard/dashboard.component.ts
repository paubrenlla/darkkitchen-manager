import { Component, inject, computed } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

import { NavbarComponent } from '../navbar/navbar.component';

interface NavItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatTooltipModule,
    NavbarComponent
  ],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent {
  private authService = inject(AuthService);
  currentUserRole = this.authService.userRole;
  currentUserName = this.authService.currentUserName;

  greeting = computed(() => {
    const name = this.currentUserName();
    const hour = new Date().getHours();
    const saludo = hour < 12 ? 'Buenos días' : hour < 19 ? 'Buenas tardes' : 'Buenas noches';
    return name ? `${saludo}, ${name}` : saludo;
  });

  currentDate = computed(() =>
    new Date().toLocaleDateString('es-UY', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    })
  );

  menuItems = computed<NavItem[]>(() => {
    const role = this.currentUserRole();
    const items: NavItem[] = [];

    if (!role) return items;

    if (role === 'Cliente') {
      items.push(
        {label: 'Ver Catálogo', icon: 'restaurant_menu', route: 'catalog'},
        {label: 'Mis Pedidos', icon: 'receipt_long', route: 'my-orders'},
        { label: 'Promociones', icon: 'auto_awesome', route: 'promotions-catalog' }
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
}
