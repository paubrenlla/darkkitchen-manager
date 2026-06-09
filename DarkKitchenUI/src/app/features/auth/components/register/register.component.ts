import { Component, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { UserService } from '../../../users/services/user.service';
import { RegisterRequest } from '../../../users/models/user.models';

import { parseBackendErrors } from '../../../../core/utils/error-parser';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    RouterLink
  ],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  name = '';
  surname = '';
  email = '';
  countryPrefix = '+598';
  phone = '';
  password = '';

  hidePassword = signal(true);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  fieldErrors = signal<Record<string, string>>({});

  constructor(private userService: UserService, private router: Router) { }

  onSubmit(): void {
    if (!this.name || !this.surname || !this.email || !this.phone || !this.password) {
      this.errorMessage.set('Por favor, completa todos los campos requeridos.');
      return;
    }

    const registerData: RegisterRequest = {
      name: this.name,
      surname: this.surname,
      email: this.email,
      countryPrefix: this.countryPrefix,
      phoneNumber: this.phone,
      password: this.password
    };

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.fieldErrors.set({});

    this.userService.postUser(registerData).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        this.router.navigate(['/login']);
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
