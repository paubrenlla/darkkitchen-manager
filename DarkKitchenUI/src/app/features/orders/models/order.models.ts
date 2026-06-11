export interface OrderAddressResponse {
  street: string;
  number: string;
  apartment: string | null;
  city: string;
  country: string;
}

export interface OrderItemDetailResponse {
  productId: string;
  productName: string;
  quantity: number;
  price: number;
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
  subtotal: number;
  discount: number;
  total: number;
  deliveryType: string;
  address: OrderAddressResponse;
  items: OrderItemDetailResponse[];
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
