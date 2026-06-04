import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterRequest, UserResponse } from '../models/user.models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'https://localhost:7180/api/user';

  constructor(private http: HttpClient) {}
  postUser(userData: RegisterRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(this.apiUrl, userData);
  }
}
