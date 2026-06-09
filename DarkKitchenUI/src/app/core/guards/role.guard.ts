import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';

export const roleGuard = (allowedRoles: string[]): CanActivateFn => {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);
    const role = authService.userRole();

    if(role && allowedRoles.includes(role)) {
      return true;
    }

    router.navigate(['/dashboard']);
    return false;
  };
};
