export interface ShippingTypeResponse {
  id: string;
  name: string;
  cost: number;
}

export interface ShippingTypeRequest {
  name: string;
  cost: number;
}
