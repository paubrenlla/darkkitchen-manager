import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterRequest, UserResponse } from '../models/user.models';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/user`;
  constructor(private http: HttpClient) { }
  postUser(userData: RegisterRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(this.apiUrl, userData);
  }
}
