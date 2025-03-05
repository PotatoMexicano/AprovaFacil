import { useIsMobile } from "@/hooks/use-mobile";
import { ColumnDef } from "@tanstack/react-table";
import { useNavigate } from "react-router-dom";

export type Request = {
  managerId: number;
  directorsIds: number[],
  companyId: number;
  paymentDate: Date,
  amount: number;
  note?: string;
  invoice: File,
  budget?: File
}

const useColumns = () => {
  const navigate = useNavigate();
  const isMobile = useIsMobile();

  const columns: ColumnDef<Request>[] = [
    {
      accessorKey: "id",
      header: "Identificador",
    },
    {
      accessorKey: "idEmpresa",
      header: "Empresa",
    },
    {
      accessorKey: "idUsuario",
      header: "Usuário",
    },
    {
      accessorKey: "idValiador",
      header: "Validador",
    },
    {
      accessorKey: "idUsuario",
      header: "Situação",
    },
    {
      accessorKey: "dataCriacao",
      header: "Data Solicitação",
    },
    {
      accessorKey: "dataPagamento",
      header: "Data Pagamento",
    },
    {
      accessorKey: "valor",
      header: "Valor",
    },
    {
      accessorKey: "observacao",
      header: "Observação",
    },
  ];
}

export default useColumns;