import { UserResponse } from "./auth";
import { CompanyResponse } from "./company";

export interface RequestReponse {
  uuid: string;
  requester_id: number;
  invoice_name: string;
  budget_name: string;
  has_invoice: boolean;
  has_budget: boolean;
  payment_date: string;
  create_at: string;
  first_level_at: string;
  approved_first_level: number;
  second_level_at: string;
  approved_second_level: number;
  approved: number;
  approved_label: string;
  level: number;
  received_at: string;
  amount: number;
  note: string;
  company: CompanyResponse;
  requester: UserResponse;
  finisher: UserResponse;
  managers: UserResponse[];
  directors: UserResponse[];
}

export interface MonthlyStatusData {
  month: string
  pending: number
  approved: number
  rejected: number
}

export interface RequestStatsResponse {
  total_requests: number
  total_request_pending: number
  total_request_approved: number
  total_request_rejected: number
  total_amount_requests_approved: number
  total_requests_by_month: MonthlyStatusData[] // atualizado!
}