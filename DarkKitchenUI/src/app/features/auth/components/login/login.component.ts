import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { parseBackendErrors } from '../../../../utils/error-parser';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  email = '';
  password = '';

  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  hidePassword = signal(true);
  fieldErrors = signal<Record<string, string>>({});

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    if (!this.email || !this.password) {
      this.errorMessage.set('Por favor, completa todos los campos.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.fieldErrors.set({});

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: (response: any) => {
        this.isLoading.set(false);

        const token = response.token || response;

        if (token) {
          this.authService.saveToken(token);
          this.router.navigate(['/dashboard']);
        } else {
          this.errorMessage.set('No se recibió un token válido del servidor.');
        }
      },
      error: (err: HttpErrorResponse) => {
        this.isLoading.set(false);

        const parsed = parseBackendErrors(err);

        this.errorMessage.set(parsed.global);
        this.fieldErrors.set(parsed.fields);
      }
    });
  }
}
