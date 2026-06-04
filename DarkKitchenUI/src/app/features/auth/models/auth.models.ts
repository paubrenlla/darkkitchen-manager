export interface LoginRequest {
  email: string;
  password: string;
}

export interface UserDto {
  id: string;
  email: string;
  role: string;
}

export interface LoginResponse {
  token: string;
  user: UserDto;
}
