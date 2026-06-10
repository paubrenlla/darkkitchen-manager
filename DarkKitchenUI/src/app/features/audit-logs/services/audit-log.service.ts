import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { AuditLogFilters, AuditLogResponse } from '../models/audit-log.models';

@Injectable({
  providedIn: 'root',
})
export class AuditLogService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/audits`;

  auditLogs = signal<AuditLogResponse[]>([]);
  isLoading = signal(false);

  getAudits(filters: AuditLogFilters): Observable<AuditLogResponse[]> {
    let params = new HttpParams().set('from', filters.from).set('to', filters.to);

    if (filters.entityName) {
      params = params.set('entityName', filters.entityName);
    }

    if (filters.entityId) {
      params = params.set('entityId', filters.entityId);
    }

    return this.http.get<AuditLogResponse[]>(this.apiUrl, { params }).pipe(
      map(result => result ?? []) // 204 No Content → null → []
    );
  }
}

