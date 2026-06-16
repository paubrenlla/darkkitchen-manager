export interface TopProductResponse {
  code: string;
  name: string;
  quantitySold: number;
  images: string[];
}

export interface SalesClientResponse {
  clientName: string;
  total: number;
}

export interface SalesPeriodResponse {
  year: number;
  month: number;
  clients: SalesClientResponse[];
  periodTotal: number;
}

export interface SalesReportResponse {
  periods: SalesPeriodResponse[];
  grandTotal: number;
}
