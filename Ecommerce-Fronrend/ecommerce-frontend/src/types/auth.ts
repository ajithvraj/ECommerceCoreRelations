export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  name: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  id: number;
  name: string;
  email: string;
  role: string;
  token: string;
  refreshToken: string;
  refreshTokenExpiry: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: object | null;
}