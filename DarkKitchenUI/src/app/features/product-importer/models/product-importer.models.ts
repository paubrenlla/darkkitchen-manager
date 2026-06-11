export interface ProductImportRequest {
  importerName: string;
  filePath: string;
}

export interface ProductImportResponse {
  totalProcessed: number;
  successful: number;
  failed: number;
  importedProducts: any[];
  errors: string[];
}
