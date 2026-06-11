import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { ReportService } from '../../services/report.service';
import { TopProductResponse } from '../../models/report.models';

@Component({
  selector: 'app-top-products-report',
  standalone: true,
  imports: [
    FormsModule,
    MatTableModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './top-products-report.component.html',
})
export class TopProductsReportComponent {
  private reportService = inject(ReportService);

  fromDate: Date | null = null;
  toDate: Date | null = null;

  displayedColumns = ['code', 'name', 'quantitySold'];
  products = signal<TopProductResponse[]>([]);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  hasSearched = signal(false);

  search(): void {
    if (!this.fromDate || !this.toDate) {
      this.errorMessage.set('Por favor, selecciona un rango de fechas.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.products.set([]);
    this.hasSearched.set(true);

    this.reportService.getTopProducts(this.fromDate, this.toDate).subscribe({
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
