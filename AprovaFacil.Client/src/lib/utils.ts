import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export const formatCurrency = (value: string | number): string => {
  // Se o valor for vazio ou indefinido, retorna string vazia
  if (value === undefined || value === null || value === "") return ""

  // Converte para número
  const numValue =
    typeof value === "string" ? Number.parseFloat(value.replace(/[^\d.,]/g, "").replace(",", ".")) || 0 : Number(value)

  // Formata como moeda brasileira
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(numValue)
}

// Função para converter string formatada em moeda para número
export const parseCurrency = (value: string): number => {
  if (!value) return 0

  // Remove símbolos de moeda, pontos e substitui vírgula por ponto
  const numericString = value
    .replace(/[^\d.,]/g, "") // Remove tudo exceto números, pontos e vírgulas
    .replace(/\./g, "") // Remove pontos (separadores de milhar)
    .replace(",", ".") // Substitui vírgula por ponto (para decimal)

  const parsedValue = Number.parseFloat(numericString)
  return isNaN(parsedValue) ? 0 : parsedValue
}
