import { z } from "zod";

export default z.object({
    id: z.number().optional().default(0),
    cnpj: z.string().min(18),
    trade_name: z.string().min(1).max(250),
    legal_name: z.string().min(1).max(250),
    postal_code: z.string(),
    state: z.string().min(3).max(30),
    city: z.string().min(3).max(100),
    street: z.string().min(3).max(250),
    neighborhood: z.string().min(3).max(100),
    number: z.string().max(10),
    complement: z.string().max(300).optional(),
    phone: z.string().min(15),
    email: z.string().email()
  })