import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  PromotionResponse,
  PromotionCreateRequest,
  PromotionUpdateRequest,
} from '../models/promotion.models';

@Injectable({
  providedIn: 'root',
})
export class PromotionService {
  private apiUrl = `${environment.apiUrl}/promotions`;

  promotions = signal<PromotionResponse[]>([]);
  isLoading = signal(false);

  constructor(private http: HttpClient) {}

  getAll(date?: string, line?: string, productCode?: string): Observable<PromotionResponse[]> {
    let params = new HttpParams();
    if (date) params = params.set('date', date);
    if (line) params = params.set('line', line);
    if (productCode) params = params.set('productCode', productCode);

    return this.http.get<PromotionResponse[]>(this.apiUrl, { params });
  }

  create(request: PromotionCreateRequest): Observable<PromotionResponse> {
    return this.http.post<PromotionResponse>(this.apiUrl, request);
  }

  update(id: string, request: PromotionUpdateRequest): Observable<PromotionResponse> {
    return this.http.put<PromotionResponse>(`${this.apiUrl}/${id}`, request);
  }
}
