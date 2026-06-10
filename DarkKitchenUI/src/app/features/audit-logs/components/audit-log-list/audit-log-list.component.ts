import { Component, OnInit, inject, signal } from '@angular/core';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuditLogResponse } from '../../models/audit-log.models';
import { AuditLogService } from '../../services/audit-log.service';

@Component({
  selector: 'app-audit-log-list',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './audit-log-list.component.html',
})
export class AuditLogListComponent implements OnInit {
  private auditLogService = inject(AuditLogService);

  auditLogs = this.auditLogService.auditLogs;
  isLoading = this.auditLogService.isLoading;
  errorMessage = signal<string | null>(null);

  fromDateTime = signal(this.buildDateTimeOffset(-7));
  toDateTime = signal(this.buildDateTimeOffset(0));
  entityName = signal('');
  entityId = signal('');

  ngOnInit(): void {
    this.loadAudits();
  }

  onFilter(): void {
    this.loadAudits();
  }

  onClearFilters(): void {
    this.fromDateTime.set(this.buildDateTimeOffset(-7));
    this.toDateTime.set(this.buildDateTimeOffset(0));
    this.entityName.set('');
    this.entityId.set('');
    this.loadAudits();
  }

  onFromDateTimeChange(value: string): void {
    this.fromDateTime.set(value);
  }

  onToDateTimeChange(value: string): void {
    this.toDateTime.set(value);
  }

  onEntityNameChange(value: string): void {
    this.entityName.set(value);
  }

  onEntityIdChange(value: string): void {
    this.entityId.set(value);
  }

  formatTimestamp(timestamp: string): string {
    return new Date(timestamp).toLocaleString('es-UY');
  }

  private loadAudits(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const entityName = this.entityName().trim();
    const entityId = this.entityId().trim();

    this.auditLogService
      .getAudits({
        from: this.toIsoDateTime(this.fromDateTime()),
        to: this.toIsoDateTime(this.toDateTime()),
        entityName: entityName || undefined,
        entityId: entityId || undefined,
      })
      .subscribe({
        next: (logs) => {
          this.auditLogService.auditLogs.set(logs);
          this.isLoading.set(false);
        },
        error: () => {
          this.errorMessage.set('No se pudieron cargar las auditorías.');
          this.isLoading.set(false);
        },
      });
  }

  private buildDateTimeOffset(daysFromToday: number): string {
    const date = new Date();
    date.setDate(date.getDate() + daysFromToday);

    const year = date.getFullYear();
    const month = `${date.getMonth() + 1}`.padStart(2, '0');
    const day = `${date.getDate()}`.padStart(2, '0');
    const hours = `${date.getHours()}`.padStart(2, '0');
    const minutes = `${date.getMinutes()}`.padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  private toIsoDateTime(localDateTime: string): string {
    return new Date(localDateTime).toISOString();
  }
}
