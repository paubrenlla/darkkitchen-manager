import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ShippingTypeRequest, ShippingTypeResponse } from '../models/shipping.models';

@Injectable({
  providedIn: 'root'
})
export class ShippingTypeService {
  private apiUrl = `${environment.apiUrl}/shippingtypes`;

  shippingTypes = signal<ShippingTypeResponse[]>([]);
  isLoading = signal(false);

  constructor(private http: HttpClient) {}

  getAll(): Observable<ShippingTypeResponse[]> {
    return this.http.get<ShippingTypeResponse[]>(this.apiUrl);
  }

  create(request: ShippingTypeRequest): Observable<ShippingTypeResponse> {
    return this.http.post<ShippingTypeResponse>(this.apiUrl, request);
  }

  update(id: string, request: ShippingTypeRequest): Observable<ShippingTypeResponse> {
    return this.http.put<ShippingTypeResponse>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
