import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { parseJwt } from '../../../utils/jwt-parser';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7180/api/auth';

  userRole = signal<string | null>(null);
  currentUserEmail = signal<string | null>(null);

  constructor(private http: HttpClient) {
    this.refreshSession();
  }

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials);
  }

  saveToken(token: string): void {
    localStorage.setItem('token', token);
    this.refreshSession();
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  logout(): void {
    localStorage.removeItem('token');
    this.userRole.set(null);
    this.currentUserEmail.set(null);
  }

  isAuthenticated(): boolean {
    return this.getToken() !== null;
  }

  private refreshSession(): void {
    const token = this.getToken();

    if (token) {
      const session = parseJwt(token);
      if (session) {
        this.userRole.set(session.role);
        this.currentUserEmail.set(session.email);
        return;
      }
    }

    this.logout();
  }
}
