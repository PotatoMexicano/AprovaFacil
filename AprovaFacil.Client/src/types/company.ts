export type Company = {
  id: number;
  cnpj: string;
  trade_name: string;
  legal_name: string;
  postal_code: string;
  state: string;
  city: string;
  street: string;
  neighborhood: string;
  number: string;
  complement?: string;
  phone: string;
  email: string;
};