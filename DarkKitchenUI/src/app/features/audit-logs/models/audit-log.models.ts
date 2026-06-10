export interface AuditLogResponse {
  id: string;
  timestamp: string;
  entityName: string;
  entityId: string;
  changeDescription: string;
  responsibleUser: string;
}

export interface AuditLogFilters {
  from: string;
  to: string;
  entityName?: string;
  entityId?: string;
}
