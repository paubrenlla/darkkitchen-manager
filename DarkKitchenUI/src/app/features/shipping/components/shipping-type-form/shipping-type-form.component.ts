import { Component, inject, signal, Inject, Optional } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpErrorResponse } from '@angular/common/http';

import { ShippingTypeService } from '../../services/shipping-type.service';
import { ShippingTypeRequest, ShippingTypeResponse } from '../../models/shipping.models';

@Component({
  selector: 'app-shipping-type-form',
  standalone: true,
  imports: [
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './shipping-type-form.component.html',
})
export class ShippingTypeFormComponent {
  private shippingTypeService = inject(ShippingTypeService);
  private dialogRef = inject(MatDialogRef<ShippingTypeFormComponent>);

  isEditMode: boolean;

  name = '';
  cost: number | null = null;

  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  constructor(@Optional() @Inject(MAT_DIALOG_DATA) public data: ShippingTypeResponse | null) {
    this.isEditMode = !!data;

    if (data) {
      this.name = data.name;
      this.cost = data.cost;
    }
  }

  onSubmit(): void {
    if (!this.name.trim() || this.cost === null || this.cost < 0) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const request: ShippingTypeRequest = {
      name: this.name.trim(),
      cost: this.cost,
    };

    const obs = this.isEditMode
      ? this.shippingTypeService.update(this.data!.id, request)
      : this.shippingTypeService.create(request);

    obs.subscribe({
      next: () => {
        this.isLoading.set(false);
        this.dialogRef.close(true);
      },
      error: (err: HttpErrorResponse) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.error || 'No se pudo guardar el tipo de envío.');
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
