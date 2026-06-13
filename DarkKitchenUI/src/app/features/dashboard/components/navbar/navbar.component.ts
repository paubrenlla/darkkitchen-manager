import { Component, inject, computed, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [MatToolbarModule, MatIconModule, MatButtonModule, MatMenuModule],
  templateUrl: './navbar.component.html'
})
export class NavbarComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  email = computed(() => this.authService.currentUserEmail() ?? '');
  role = computed(() => this.authService.userRole() ?? '');

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
