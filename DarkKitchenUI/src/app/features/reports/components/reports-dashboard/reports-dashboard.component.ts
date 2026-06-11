import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { TopProductsReportComponent } from '../top-products-report/top-products-report.component';
import { SalesReportComponent } from '../sales-report/sales-report.component';

@Component({
  selector: 'app-reports-dashboard',
  standalone: true,
  imports: [
    MatTabsModule,
    MatIconModule,
    TopProductsReportComponent,
    SalesReportComponent
  ],
  templateUrl: './reports-dashboard.component.html',
})
export class ReportsDashboardComponent {}
