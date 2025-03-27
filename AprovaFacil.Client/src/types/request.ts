import { UserResponse } from "./auth";

export interface RequestReponse {
  uuid: string;
  requester_id: number;
  invoice_name: string;
  budget_name: string;
  payment_date: string;
  create_at: string;
  approved_first_level_at: string;
  approved_second_level_at: string;
  received_at: string;
  amount: number;
  note: string;
  requester: UserResponse;
  managers: UserResponse[];
  directors: UserResponse[];
}