export interface UserResponse {
  id: string;
  name: string;
  surname: string;
  email: string;
  role: string;
  phoneNumber: string;
}

export interface UserCreateRequest {
  name: string;
  surname: string;
  email: string;
  countryPrefix: string;
  phoneNumber: string;
  password: string;
  role?: string;
}

export interface UserUpdateRequest {
  name: string;
  surname: string;
  email: string;
  countryPrefix: string;
  phoneNumber: string;
  role: string;
}
