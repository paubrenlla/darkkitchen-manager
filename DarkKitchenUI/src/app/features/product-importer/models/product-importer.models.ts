import { ProductResponse } from '../../products/models/product.models';
export interface ProductImportRequest {
  importerName: string;
  filePath: string;
}

export interface ProductImportResponse {
  totalProcessed: number;
  successful: number;
  failed: number;
  importedProducts: ProductResponse[];
  errors: string[];
}
