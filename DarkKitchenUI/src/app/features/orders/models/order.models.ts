export interface OrderAddressResponse {
  street: string;
  number: string;
  apartment: string | null;
  city: string;
  country: string;
}

export interface OrderItemDetailResponse {
  productId: string;
  productName: string | null;
  quantity: number;
  price: number;
  appliedPromotion: string | null;
  itemTotal: number;
}

export interface OrderListResponse {
  id: string;
  orderNumber: number;
  clientId: string;
  clientName: string;
  createdAt: string;
  status: string;
  total: number;
  productCount: number;
}

export interface OrderDetailResponse {
  id: string;
  orderNumber: number;
  clientId: string;
  createdAt: string;
  status: string;
  deliveryType: string;
  address: OrderAddressResponse;
  items: OrderItemDetailResponse[];
  subtotal: number;
  shippingCost: number;
  tax: number;
  total: number;
}

export interface OrderStatusUpdateRequest {
  status: string;
}

export interface OrderFilter {
  fromDate?: string;
  toDate?: string;
  status?: string;
  address?: string;
}
