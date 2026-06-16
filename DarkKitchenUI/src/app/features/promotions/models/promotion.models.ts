export interface PromotionResponse {
  id: string;
  name: string;
  discountPercentage: number;
  startDate: string;
  endDate: string;
  products: string[];
}

export interface PromotionCreateRequest {
  name: string;
  discountPercentage: number;
  startDate: string;
  endDate: string;
  productCodes: string[];
}

export interface PromotionUpdateRequest {
  name: string;
  discountPercentage: number;
  startDate: string;
  endDate: string;
  productCodes: string[];
}
