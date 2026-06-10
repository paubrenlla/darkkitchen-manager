import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  ProductResponse,
  ProductCreateRequest,
  ProductUpdateRequest,
} from '../models/product.models';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = `${environment.apiUrl}/products`;

  products = signal<ProductResponse[]>([]);
  isLoading = signal(false);

  constructor(private http: HttpClient) {}

  getAll(name?: string, line?: string, category?: string): Observable<ProductResponse[]> {
    let params = new HttpParams();
    if (name) params = params.set('name', name);
    if (line) params = params.set('line', line);
    if (category) params = params.set('category', category);

    return this.http.get<ProductResponse[]>(this.apiUrl, { params });
  }

  create(request: ProductCreateRequest): Observable<ProductResponse> {
    return this.http.post<ProductResponse>(this.apiUrl, request);
  }

  update(id: string, request: ProductUpdateRequest): Observable<ProductResponse> {
    return this.http.put<ProductResponse>(`${this.apiUrl}/${id}`, request);
  }

  toggleActive(product: ProductResponse): Observable<ProductResponse> {
    const request: ProductUpdateRequest = {
      name: product.name,
      description: product.description,
      line: product.line,
      category: product.category,
      price: product.price,
      images: product.images.map((img) => ({
        url: img.url,
        sizeInBytes: img.sizeInBytes,
      })),
      isActive: !product.isActive,
    };
    return this.update(product.id, request);
  }
}
