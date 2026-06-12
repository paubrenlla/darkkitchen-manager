import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ShippingTypeResponse } from '../models/shipping.models';

@Injectable({
  providedIn: 'root'
})
export class ShippingTypeService {
  private apiUrl = `${environment.apiUrl}/shippingtypes`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ShippingTypeResponse[]> {
    return this.http.get<ShippingTypeResponse[]>(this.apiUrl);
  }
}
