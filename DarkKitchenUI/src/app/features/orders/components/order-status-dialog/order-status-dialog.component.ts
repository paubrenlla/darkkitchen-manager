import { Component, Inject, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpErrorResponse } from '@angular/common/http';

import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-status-dialog',
  standalone: true,
  imports: [
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './order-status-dialog.component.html'
})
export class OrderStatusDialogComponent {
  private orderService = inject(OrderService);
  private dialogRef = inject(MatDialogRef<OrderStatusDialogComponent>);

  selectedStatus = '';
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  readonly statusOptions = [
    { value: 'Preparado',   label: 'Preparado' },
    { value: 'Demorado',    label: 'Demorado' },
    { value: 'EnCamino',    label: 'En camino' },
    { value: 'Entregado',   label: 'Entregado' },
    { value: 'NoEntregado', label: 'No entregado' }
  ];

  constructor(@Inject(MAT_DIALOG_DATA) public orderId: string) {}

  confirm(): void {
    if(!this.selectedStatus) {
      this.errorMessage.set('Seleccioná un estado.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.orderService.updateStatus(this.orderId, this.selectedStatus).subscribe({
      next: (updated) => {
        this.isLoading.set(false);
        this.dialogRef.close(updated.status);
      },
      error: (err: HttpErrorResponse) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.error || 'No se pudo actualizar el estado.');
      }
    });
  }

  cancel(): void {
    this.dialogRef.close(null);
  }
}
