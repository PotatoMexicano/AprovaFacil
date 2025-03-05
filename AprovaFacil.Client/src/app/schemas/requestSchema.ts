import { z } from "zod";

export default z.object({
  id: z.string().optional().default('0'),
    companyId: z.number().min(1, { message: "Selecione a empresa" }),
    paymentDate: z.date()
      .refine(date => !isNaN(date.getTime()), { message: "Data inválida" })
      .refine(date => date > new Date(), { message: "A data deve ser posterior a hoje" }),
    amount: z.number()
    .min(1, "O valor deve ser maior que zero")
    .max(100000000, "O valor máximo permitido é R$ 1.000.000,00"),
    note: z.string().optional(),
    invoice: z
      .instanceof(File)
      .refine((file) => !file || file.size <= 20 * 1024 * 1024, { message: "Nota fiscal precisa ser menor que 20MB" })
      .refine((file) => file.size > 0, { message: "Nota fiscal é obrigatória" })
      .refine((file) => file.type === "application/pdf", { message: "Somente arquivos PDF são permitidos" }),
    budget: z.instanceof(File).optional(),
    managerId: z.number().min(1),
    directorsIds: z.array(z.number().min(1)),
});