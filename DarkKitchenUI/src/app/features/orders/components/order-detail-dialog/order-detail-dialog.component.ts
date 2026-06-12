import { Component, inject, signal, Inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';

import { OrderService } from '../../services/order.service';
import { OrderDetailResponse } from '../../models/order.models';

@Component({
  selector: 'app-order-detail-dialog',
  standalone: true,
  imports: [
    DatePipe,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDividerModule
  ],
  templateUrl: './order-detail-dialog.component.html'
})
export class OrderDetailDialogComponent {
  private orderService = inject(OrderService);

  order = signal<OrderDetailResponse | null>(null);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  constructor(
    @Inject(MAT_DIALOG_DATA) public orderId: string,
    private dialogRef: MatDialogRef<OrderDetailDialogComponent>
  ) {
    this.orderService.getById(orderId).subscribe({
      next: (data) => {
        this.order.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudo cargar el detalle del pedido.');
        this.isLoading.set(false);
      }
    });
  }

  getStatusClass(status: string): string {
    const classes: Record<string, string> = {
      'Pending':      'bg-amber-50 text-amber-700 border border-amber-100',
      'Prepared':     'bg-blue-50 text-blue-700 border border-blue-100',
      'Delayed':      'bg-orange-50 text-orange-700 border border-orange-100',
      'Shipping':     'bg-indigo-50 text-indigo-700 border border-indigo-100',
      'Delivered':    'bg-emerald-50 text-emerald-700 border border-emerald-100',
      'NotDelivered': 'bg-slate-50 text-slate-600 border border-slate-200',
      'Cancelled':    'bg-rose-50 text-rose-600 border border-rose-100',
    };
    return classes[status] ?? 'bg-slate-50 text-slate-600 border border-slate-200';
  }

  close(): void {
    this.dialogRef.close();
  }
}
