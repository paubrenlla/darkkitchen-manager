import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { OrderService } from '../../services/order.service';
import { OrderListResponse, OrderFilter } from '../../models/order.models';
import { OrderDetailDialogComponent } from '../order-detail-dialog/order-detail-dialog.component';
import { OrderStatusDialogComponent } from '../order-status-dialog/order-status-dialog.component';

@Component({
  selector: 'app-kitchen-orders',
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
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDialogModule
  ],
  templateUrl: './kitchen-orders.component.html'
})
export class KitchenOrdersComponent {
  private orderService = inject(OrderService);
  private dialog = inject(MatDialog);

  orders = this.orderService.orders;
  isLoading = this.orderService.isLoading;
  errorMessage = signal<string | null>(null);
  hasSearched = signal(false);

  fromDate = '';
  toDate = '';
  statusFilter = '';
  addressFilter = '';

  readonly statusOptions = [
    'Pending', 'Prepared', 'Delayed',
    'Shipping', 'Delivered', 'NotDelivered', 'Cancelled'
  ];

  displayedColumns = ['orderNumber', 'client', 'date', 'status', 'items', 'actions'];

  search(): void {
    if(!this.fromDate || !this.toDate) {
      this.errorMessage.set('El rango de fechas es obligatorio.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.hasSearched.set(true);

    const filter: OrderFilter = {
      fromDate: this.fromDate,
      toDate: this.toDate,
      status: this.statusFilter || undefined,
      address: this.addressFilter || undefined
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
    this.addressFilter = '';
    this.errorMessage.set(null);
    this.hasSearched.set(false);
    this.orderService.orders.set([]);
  }

  openDetail(order: OrderListResponse): void {
    this.dialog.open(OrderDetailDialogComponent, { data: order.id });
  }

  openStatusDialog(order: OrderListResponse): void {
    const dialogRef = this.dialog.open(OrderStatusDialogComponent, {
      data: order.id,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(newStatus => {
      if(newStatus) {
        this.orderService.orders.update(list =>
          list.map(o => o.id === order.id ? { ...o, status: newStatus } : o)
        );
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
}
