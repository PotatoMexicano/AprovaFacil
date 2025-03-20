import {z} from "zod"

export default z.object({
  full_name: z.string().min(3, { message: "Nome completo deve ter no mínimo 3 caracteres" }),
  role: z.string().min(1, { message: "Selecione um cargo" }),
  department: z.string().min(1, { message: "Selecione um departamento" }),
  picture_url: z.string().min(1, { message: "URL da imagem inválida" }).optional().default(""),
  email: z.string().email({ message: "Email inválido" }),
  password: z.string().min(6, { message: "Senha deve ter no mínimo 6 caracteres" })
})