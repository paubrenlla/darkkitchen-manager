import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { ReportService } from '../../services/report.service';
import { TopProductResponse } from '../../models/report.models';

@Component({
  selector: 'app-top-products-report',
  standalone: true,
  imports: [
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './top-products-report.component.html',
})
export class TopProductsReportComponent {
  private reportService = inject(ReportService);

  fromDate = '';
  toDate = '';

  displayedColumns = ['code', 'name', 'quantitySold'];
  products = signal<TopProductResponse[]>([]);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  hasSearched = signal(false);

  onFromDateChange(event: Event): void {
    this.fromDate = (event.target as HTMLInputElement).value;
  }

  onToDateChange(event: Event): void {
    this.toDate = (event.target as HTMLInputElement).value;
  }
  search(): void {
    if (!this.fromDate || !this.toDate) {
      this.errorMessage.set('Por favor, selecciona un rango de fechas.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.products.set([]);
    this.hasSearched.set(true);

    const from = new Date(this.fromDate);
    const to = new Date(this.toDate);

    this.reportService.getTopProducts(from, to).subscribe({
      next: (data) => {
        this.products.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Ocurrió un error al cargar el reporte.');
        this.isLoading.set(false);
      }
    });
  }
}
