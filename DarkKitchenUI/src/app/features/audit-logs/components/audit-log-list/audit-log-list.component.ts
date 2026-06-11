import { Component, OnInit, computed, inject, signal } from '@angular/core';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuditLogService } from '../../services/audit-log.service';

type SortableColumn = 'timestamp' | 'entityName' | 'responsibleUser';
type SortDirection = 'asc' | 'desc';

@Component({
  selector: 'app-audit-log-list',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './audit-log-list.component.html',
})
export class AuditLogListComponent implements OnInit {
  private auditLogService = inject(AuditLogService);
  private missingDateRangeMessage = 'Debes indicar tanto "Desde" como "Hasta".';
  private invalidDateRangeMessage = 'El rango es inválido: "Desde" no puede ser mayor que "Hasta".';

  auditLogs = this.auditLogService.auditLogs;
  isLoading = this.auditLogService.isLoading;
  errorMessage = signal<string | null>(null);

  fromDateTime = signal(this.buildDateTimeOffset(-7));
  toDateTime = signal(this.buildDateTimeOffset(0));
  entityName = signal('');
  entityId = signal('');

  // Paginado
  page = signal(1);
  pageSize = signal(20);
  readonly pageSizeOptions = [10, 20, 50, 100];

  // Ordenado
  sortBy = signal<SortableColumn>('timestamp');
  sortDir = signal<SortDirection>('desc');

  // Logs ordenados (client-side)
  sortedLogs = computed(() => {
    const logs = [...this.auditLogs()];
    const col = this.sortBy();
    const dir = this.sortDir();

    return logs.sort((a, b) => {
      let valA: string | number;
      let valB: string | number;

      if (col === 'timestamp') {
        valA = new Date(a.timestamp).getTime();
        valB = new Date(b.timestamp).getTime();
      } else {
        valA = a[col].toLowerCase();
        valB = b[col].toLowerCase();
      }

      if (valA < valB) return dir === 'asc' ? -1 : 1;
      if (valA > valB) return dir === 'asc' ? 1 : -1;
      return 0;
    });
  });

  // Slice de la página actual
  pagedLogs = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.sortedLogs().slice(start, start + this.pageSize());
  });

  totalPages = computed(() =>
    Math.max(1, Math.ceil(this.auditLogs().length / this.pageSize()))
  );

  canGoPrev = computed(() => this.page() > 1);
  canGoNext = computed(() => this.page() < this.totalPages());

  isDateRangeInvalid = computed(() => {
    const from = this.fromDateTime();
    const to = this.toDateTime();

    if (!from || !to) {
      return false;
    }

    return new Date(from).getTime() > new Date(to).getTime();
  });

  isDateRangeMissing = computed(() => {
    const from = this.fromDateTime().trim();
    const to = this.toDateTime().trim();
    return !from || !to;
  });

  validationMessage = computed(() => {
    if (this.isDateRangeMissing()) {
      return this.missingDateRangeMessage;
    }

    if (this.isDateRangeInvalid()) {
      return this.invalidDateRangeMessage;
    }

    return null;
  });

  isFilterDisabled = computed(() => this.isLoading() || this.validationMessage() !== null);

  ngOnInit(): void {
    this.loadAudits();
  }

  onFilter(): void {
    const validationError = this.validationMessage();
    if (validationError) {
      this.errorMessage.set(validationError);
      return;
    }

    this.page.set(1);
    this.loadAudits();
  }

  onClearFilters(): void {
    this.fromDateTime.set(this.buildDateTimeOffset(-7));
    this.toDateTime.set(this.buildDateTimeOffset(0));
    this.entityName.set('');
    this.entityId.set('');
    this.page.set(1);
    this.loadAudits();
  }

  onFromDateTimeChange(value: string): void {
    this.fromDateTime.set(value);
    this.syncValidationErrorMessage();
  }

  onToDateTimeChange(value: string): void {
    this.toDateTime.set(value);
    this.syncValidationErrorMessage();
  }

  onEntityNameChange(value: string): void {
    this.entityName.set(value);
  }

  onEntityIdChange(value: string): void {
    this.entityId.set(value);
  }

  onSort(column: SortableColumn): void {
    if (this.sortBy() === column) {
      this.sortDir.update(d => (d === 'asc' ? 'desc' : 'asc'));
    } else {
      this.sortBy.set(column);
      this.sortDir.set('asc');
    }
    this.page.set(1);
  }

  onPrevPage(): void {
    if (this.canGoPrev()) {
      this.page.update(p => p - 1);
    }
  }

  onNextPage(): void {
    if (this.canGoNext()) {
      this.page.update(p => p + 1);
    }
  }

  onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.page.set(1);
  }

  formatTimestamp(timestamp: string): string {
    return new Date(timestamp).toLocaleString('es-UY');
  }

  private loadAudits(): void {
    const validationError = this.validationMessage();
    if (validationError) {
      this.errorMessage.set(validationError);
      return;
    }

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

  private syncValidationErrorMessage(): void {
    const validationError = this.validationMessage();

    if (validationError) {
      this.errorMessage.set(validationError);
      return;
    }

    const currentError = this.errorMessage();
    if (
      currentError === this.missingDateRangeMessage ||
      currentError === this.invalidDateRangeMessage
    ) {
      this.errorMessage.set(null);
    }
  }
}
