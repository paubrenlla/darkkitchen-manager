import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ProductImportRequest, ProductImportResponse } from '../models/product-importer.models';

@Injectable({
  providedIn: 'root'
})
export class ProductImporterService {
  private apiUrl = `${environment.apiUrl}/products/import`;
  private pluginsUrl = `${environment.apiUrl}/plugins/importers`;

  constructor(private http: HttpClient) {}

  getImporters(): Observable<string[]> {
    return this.http.get<string[]>(this.pluginsUrl);
  }

  importProducts(request: ProductImportRequest): Observable<ProductImportResponse> {
    return this.http.post<ProductImportResponse>(this.apiUrl, request);
  }
}
