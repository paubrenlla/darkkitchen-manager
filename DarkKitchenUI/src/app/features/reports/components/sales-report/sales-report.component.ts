import { Component, inject, signal, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

import { ReportService } from '../../services/report.service';
import { SalesReportResponse } from '../../models/report.models';

@Component({
  selector: 'app-sales-report',
  standalone: true,
  imports: [
    DecimalPipe,
    MatExpansionModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './sales-report.component.html',
})
export class SalesReportComponent implements OnInit {
  private reportService = inject(ReportService);

  report = signal<SalesReportResponse | null>(null);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadReport();
  }

  loadReport(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.reportService.getSalesReport().subscribe({
      next: (data) => {
        this.report.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Ocurrió un error al cargar el reporte de ventas.');
        this.isLoading.set(false);
      }
    });
  }

  getMonthName(month: number): string {
    const date = new Date(2000, month - 1, 1);
    const name = date.toLocaleString('es-ES', { month: 'long' });
    return name.charAt(0).toUpperCase() + name.slice(1);
  }
}
