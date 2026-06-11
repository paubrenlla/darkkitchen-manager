import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { DatePipe } from '@angular/common';

import { OrderService } from '../../services/order.service';
import { OrderDetailResponse } from '../../models/order.models';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDividerModule,
  ],
  templateUrl: './admin-orders.component.html',
})
export class AdminOrdersComponent {
  private orderService = inject(OrderService);

  orderId = '';
  order = signal<OrderDetailResponse | null>(null);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  hasSearched = signal(false);

  search(): void {
    if (!this.orderId.trim()) {
      this.errorMessage.set('Ingresá el ID del pedido.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.order.set(null);
    this.hasSearched.set(true);

    this.orderService.getById(this.orderId.trim()).subscribe({
      next: (data) => {
        this.order.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || 'Pedido no encontrado.');
        this.isLoading.set(false);
      },
    });
  }

  clear(): void {
    this.orderId = '';
    this.order.set(null);
    this.errorMessage.set(null);
    this.hasSearched.set(false);
  }

  getStatusClass(status: string): string {
    const classes: Record<string, string> = {
      Pending: 'bg-amber-50 text-amber-700 border border-amber-100',
      Prepared: 'bg-blue-50 text-blue-700 border border-blue-100',
      Delayed: 'bg-orange-50 text-orange-700 border border-orange-100',
      Shipping: 'bg-indigo-50 text-indigo-700 border border-indigo-100',
      Delivered: 'bg-emerald-50 text-emerald-700 border border-emerald-100',
      NotDelivered: 'bg-slate-50 text-slate-600 border border-slate-200',
      Cancelled: 'bg-rose-50 text-rose-600 border border-rose-100',
    };
    return classes[status] ?? 'bg-slate-50 text-slate-600 border border-slate-200';
  }
}
