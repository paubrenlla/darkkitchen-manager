import { Component, inject, signal, Inject, Optional } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpErrorResponse } from '@angular/common/http';

import { UserService } from '../../services/user.service';
import { UserResponse, UserCreateRequest, UserUpdateRequest } from '../../models/user.models';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './user-form.component.html',
})
export class UserFormComponent {
  private userService = inject(UserService);
  private dialogRef = inject(MatDialogRef<UserFormComponent>);

  isEditMode: boolean;

  name = '';
  surname = '';
  email = '';
  countryPrefix = '';
  phoneNumber = '';
  password = '';
  role = '';

  readonly roles = ['Administrativo', 'Preparador', 'Cliente'];

  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  constructor(@Optional() @Inject(MAT_DIALOG_DATA) public data: UserResponse | null) {
    this.isEditMode = !!data;
    if (data) {
      this.name = data.name;
      this.surname = data.surname;
      this.email = data.email;
      this.role = data.role;
      const parsed = this.parsePhone(data.phoneNumber);
      this.countryPrefix = parsed.prefix;
      this.phoneNumber = parsed.number;
    }
  }

  private parsePhone(combined: string): { prefix: string; number: string } {
    const match = combined.match(/^(\+\d{1,3})(.+)$/);
    if (match) {
      return { prefix: match[1], number: match[2] };
    }
    return { prefix: '+598', number: combined };
  }

  onSubmit(): void {
    if (!this.isFormValid()) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const obs = this.isEditMode
      ? this.userService.update(this.data!.id, this.buildUpdateRequest())
      : this.userService.create(this.buildCreateRequest());

    obs.subscribe({
      next: () => {
        this.isLoading.set(false);
        this.dialogRef.close(true);
      },
      error: (err: HttpErrorResponse) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.error || 'No se pudo guardar el usuario.');
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  private buildCreateRequest(): UserCreateRequest {
    return {
      name: this.name.trim(),
      surname: this.surname.trim(),
      email: this.email.trim(),
      countryPrefix: this.countryPrefix.trim(),
      phoneNumber: this.phoneNumber.trim(),
      password: this.password,
      role: this.role || undefined,
    };
  }

  private buildUpdateRequest(): UserUpdateRequest {
    return {
      name: this.name.trim(),
      surname: this.surname.trim(),
      email: this.email.trim(),
      countryPrefix: this.countryPrefix.trim() || '+598',
      phoneNumber: this.phoneNumber.trim(),
      role: this.role,
    };
  }

  private isFormValid(): boolean {
    if (!this.name || !this.surname || !this.email) {
      this.errorMessage.set('Nombre, apellido y email son obligatorios.');
      return false;
    }
    if (!this.isEditMode && (!this.countryPrefix || !this.phoneNumber || !this.password)) {
      this.errorMessage.set('Completá todos los campos obligatorios.');
      return false;
    }
    if (this.isEditMode && !this.role) {
      this.errorMessage.set('El rol es obligatorio.');
      return false;
    }
    return true;
  }
}
