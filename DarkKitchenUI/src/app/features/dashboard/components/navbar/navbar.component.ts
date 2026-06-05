import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, MatToolbarModule, MatIconModule, MatButtonModule],
  templateUrl: './navbar.component.html'
})
export class NavbarComponent {
  readonly toolbarClasses = 'flex justify-between items-center px-6 font-sans shadow-md w-full h-16';
  readonly welcomeTextClasses = 'text-sm font-normal tracking-wide opacity-95';
  readonly logoutButtonClasses = 'hover:bg-white/10 transition-colors rounded-full';

  private authService = inject(AuthService);
  private router = inject(Router);

  displayName = computed(() => {
    return this.authService.currentUserName() || 'Usuario';
  });

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
