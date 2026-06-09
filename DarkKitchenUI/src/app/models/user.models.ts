export interface RegisterRequest {
  name: string;
  surname: string;
  email: string;
  countryPrefix: string;
  phoneNumber: string;
  password: string;
}

export interface UserResponse {
  id: string;
  name: string;
  surname: string;
  email: string;
  phone: string;
  role: string;
}
