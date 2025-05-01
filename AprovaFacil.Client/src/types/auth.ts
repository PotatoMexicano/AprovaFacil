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
  request_approved: number;
  tenant_id: number;
  tenant_name: string | null;
}

export interface UserNotification {
  uuid: string;
  create_at: Date;
  expire_at: Date;
  message: string;
  user_id: number
  read: boolean;
  request_uuid: string;
}

export interface LoginResponse {
  message: string;
  user: UserResponse;
}