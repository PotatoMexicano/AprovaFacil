"use client"

import { useEffect } from "react"
import { AlertCircle } from "lucide-react"
import { Badge } from "@/app/components/ui/badge"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card"
import { useBreadcrumb } from "@/app/context/breadkcrumb-context"
import { Skeleton } from "@/app/components/ui/skeleton"
import { Alert, AlertDescription, AlertTitle } from "@/app/components/ui/alert"
import { Requisicao } from "@/types/request"
import useColumns from "./columns"
import { useGetCompaniesQuery } from "@/app/api/companyApiSlice"
import { DataTable } from "../../company/view-companies/data-table"

// Dados de exemplo
const requisicoes: Requisicao[] = [
  {
    id: "REQ-001",
    notaFiscal: {
      nome: "NF-2023-001.pdf",
      url: "#",
    },
    orcamento: {
      nome: "ORC-2023-001.pdf",
      url: "#",
    },
    validador: "João Silva",
    empresa: "Tech Solutions Ltda",
    dataPagamento: new Date(2023, 11, 15),
    valor: 5000.0,
    observacao: "Serviços de consultoria em TI",
    status: "aprovado",
  },
  {
    id: "REQ-002",
    notaFiscal: {
      nome: "NF-2023-002.pdf",
      url: "#",
    },
    orcamento: {
      nome: "ORC-2023-002.pdf",
      url: "#",
    },
    validador: "Maria Oliveira",
    empresa: "Construções Rápidas S.A.",
    dataPagamento: new Date(2023, 11, 20),
    valor: 12500.0,
    observacao: "Materiais de construção para reforma",
    status: "pendente",
  },
  {
    id: "REQ-003",
    notaFiscal: {
      nome: "NF-2023-003.pdf",
      url: "#",
    },
    orcamento: {
      nome: "ORC-2023-003.pdf",
      url: "#",
    },
    validador: "Carlos Mendes",
    empresa: "Distribuidora Global",
    dataPagamento: new Date(2023, 10, 30),
    valor: 3200.0,
    observacao: "Fornecimento de equipamentos de escritório",
    status: "recusado",
  },
  {
    id: "REQ-004",
    notaFiscal: {
      nome: "NF-2023-004.pdf",
      url: "#",
    },
    orcamento: {
      nome: "ORC-2023-004.pdf",
      url: "#",
    },
    validador: "Ana Beatriz",
    empresa: "Consultoria Financeira Ltda",
    dataPagamento: new Date(2023, 9, 10),
    valor: 7800.0,
    observacao: "Serviços de auditoria financeira",
    status: "expirado",
  },
  {
    id: "REQ-005",
    notaFiscal: {
      nome: "NF-2023-005.pdf",
      url: "#",
    },
    orcamento: {
      nome: "ORC-2023-005.pdf",
      url: "#",
    },
    validador: "Roberto Alves",
    empresa: "Indústrias Unidas S.A.",
    dataPagamento: new Date(2023, 11, 28),
    valor: 15000.0,
    observacao: "Manutenção de maquinário industrial",
    status: "aprovado",
  },
]

// Adicionar estado para controlar a visualização de anexos
export default function ViewRequestsPage() {
  const { setBreadcrumbs } = useBreadcrumb();

  useEffect(() => {
    setBreadcrumbs(["Início", "Requisições"]); // Define os breadcrumbs da página atual
  }, [setBreadcrumbs]);

  const { data: company, error: errorCompany, isError: isCompanyError, isLoading: isCompanyLoading } = useGetCompaniesQuery();
  const columns = useColumns();

  return (
    <>
      <Card className="col-span-12 flex flex-col shadow-none border-0">
        <CardHeader>
          <CardTitle className="text-2xl w-full flex justify-between">
            Empresas cadastradas
            <Badge className="m-2" variant={"default"}> {isCompanyLoading ? "..." : company?.length} Empresas </Badge>
          </CardTitle>
          <CardDescription>Empresas cadastradas para emissão de nota fiscal.</CardDescription>
        </CardHeader>
        <CardContent>
          {isCompanyLoading
            ? (
              <div className="flex flex-col space-y-3 py-4">
                <div className="space-y-2">
                  <Skeleton className="h-9 max-w-sm" />
                </div>
                <Skeleton className="h-[35rem] w-full rounded-xl" />
              </div>
            )
            : (
              !isCompanyError && company
                ? (
                  <DataTable columns={columns} data={company} />
                )
                : (
                  <Alert variant="destructive">
                    <AlertCircle className="h-4 w-4" />
                    <AlertTitle>Error</AlertTitle>
                    <AlertDescription>
                      {(errorCompany as Error).message}
                    </AlertDescription>
                  </Alert>
                )
            )}
        </CardContent>
      </Card>

    </>
  )
}

