import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  OrderListResponse,
  OrderDetailResponse,
  OrderStatusUpdateRequest,
  OrderFilter
} from '../models/order.models';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = `${environment.apiUrl}/orders`;

  orders = signal<OrderListResponse[]>([]);
  isLoading = signal(false);

  constructor(private http: HttpClient) {}

  getAll(filter: OrderFilter = {}): Observable<OrderListResponse[]> {
    let params = new HttpParams();
    if(filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if(filter.toDate)   params = params.set('toDate', filter.toDate);
    if(filter.status)   params = params.set('status', filter.status);
    if(filter.address)  params = params.set('address', filter.address);

    return this.http.get<OrderListResponse[]>(this.apiUrl, { params });
  }

  getById(id: string): Observable<OrderDetailResponse> {
    return this.http.get<OrderDetailResponse>(`${this.apiUrl}/${id}`);
  }

  updateStatus(id: string, status: string): Observable<OrderDetailResponse> {
    const request: OrderStatusUpdateRequest = { status };
    return this.http.patch<OrderDetailResponse>(`${this.apiUrl}/${id}/status`, request);
  }
}
