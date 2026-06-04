import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7180/api/auth';

  private tokenSignal = signal<string | null>(localStorage.getItem('dk_token'));
  public isAuthenticated = computed(() => !!this.tokenSignal());
  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials);
  }

  getToken(): string | null {
    return this.tokenSignal();
  }

  saveToken(token: string): void {
    localStorage.setItem('dk_token', token);
    this.tokenSignal.set(token);
  }

  logout(): void {
    localStorage.removeItem('dk_token');
    this.tokenSignal.set(null);
  }
}
