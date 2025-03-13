// Tipos para as requisições
export type StatusType = "aprovado" | "pendente" | "recusado" | "expirado"

export interface Requisicao {
  id: string
  notaFiscal: {
    nome: string
    url: string
  }
  orcamento: {
    nome: string
    url: string
  }
  validador: string
  empresa: string
  dataPagamento: Date
  valor: number
  observacao: string
  status: StatusType
}