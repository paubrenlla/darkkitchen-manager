import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { parseJwt } from '../../../core/utils/jwt-parser';
import { environment } from '../../../../environments/environment';
import { LoginRequest, LoginResponse } from '../models/auth.models';
import { tap, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  userRole = signal<string | null>(null);
  currentUserEmail = signal<string | null>(null);
  currentUserName = signal<string | null>(null);

  constructor(private http: HttpClient) {
    this.refreshSession();
  }

  login(credentials: LoginRequest): Observable<void> {
  return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
    tap(response => {
      localStorage.setItem('token', response.token);
      this.refreshSession();
    }),
    map(() => void 0)
  );
}

  private saveToken(token: string): void {
    localStorage.setItem('token', token);
    this.refreshSession();
  }

  private getToken(): string | null {
    return localStorage.getItem('token');
  }

  getAuthToken(): string | null {
    return this.getToken();
  }

  logout(): void {
    localStorage.removeItem('token');
    this.userRole.set(null);
    this.currentUserEmail.set(null);
    this.currentUserName.set(null);
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
        this.currentUserName.set(session.name);
        return;
      }
    }

    this.logout();
  }
}
