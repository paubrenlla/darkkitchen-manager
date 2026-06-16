import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TopProductResponse, SalesReportResponse } from '../models/report.models';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/reports`;

  getTopProducts(fromDate: Date, toDate: Date): Observable<TopProductResponse[]> {
    const params = new HttpParams()
      .set('fromDate', fromDate.toISOString())
      .set('toDate', toDate.toISOString());
    return this.http.get<TopProductResponse[]>(`${this.apiUrl}/top-products`, { params });
  }

  getSalesReport(): Observable<SalesReportResponse> {
    return this.http.get<SalesReportResponse>(`${this.apiUrl}/sales`);
  }
}
