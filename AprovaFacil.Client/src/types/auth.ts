export interface LoginRequest {
  email: string;
  password: string;
}

export interface UserResponse {
  id: number;
  email: string;
  full_name: string;
  role_label: string;
  role: string;
  department: string;
  department_label: string;
  picture_url: string;
  enabled: boolean;
  identity_roles: string[];
}

export interface LoginResponse {
  message: string;
  user: UserResponse;
}