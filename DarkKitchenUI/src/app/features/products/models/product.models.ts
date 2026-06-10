export interface ProductImageResponse {
  url: string;
  sizeInBytes: number;
}

export interface ProductResponse {
  id: string;
  code: string;
  name: string;
  description: string;
  price: number;
  line: string;
  category: string;
  images: ProductImageResponse[];
  isActive: boolean;
}

export interface ProductImageDto {
  url: string;
  sizeInBytes: number;
}

export interface ProductCreateRequest {
  code: string;
  name: string;
  description: string;
  line: string;
  category: string;
  price: number;
  images: ProductImageDto[];
}

export interface ProductUpdateRequest {
  name: string;
  description: string;
  line: string;
  category: string;
  price: number;
  images: ProductImageDto[];
  isActive?: boolean;
}
