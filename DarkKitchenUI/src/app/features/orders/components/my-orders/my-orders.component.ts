import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { OrderService } from '../../services/order.service';
import { OrderFilter } from '../../models/order.models';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [
    FormsModule,
    DatePipe,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './my-orders.component.html'
})
export class MyOrdersComponent implements OnInit {
  private orderService = inject(OrderService);

  orders = this.orderService.orders;
  isLoading = this.orderService.isLoading;
  errorMessage = signal<string | null>(null);

  fromDate = '';
  toDate = '';
  statusFilter = '';

  readonly statusOptions = [
    'Pending', 'Prepared', 'Delayed',
    'Shipping', 'Delivered', 'NotDelivered', 'Cancelled'
  ];

  displayedColumns = ['orderNumber', 'client', 'date', 'status', 'total', 'items'];

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const filter: OrderFilter = {
      fromDate: this.fromDate || undefined,
      toDate:   this.toDate   || undefined,
      status:   this.statusFilter || undefined
    };

    this.orderService.getAll(filter).subscribe({
      next: (data) => {
        this.orderService.orders.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || 'No se pudieron cargar los pedidos.');
        this.isLoading.set(false);
      }
    });
  }

  clearFilters(): void {
    this.fromDate = '';
    this.toDate = '';
    this.statusFilter = '';
    this.loadOrders();
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
}
