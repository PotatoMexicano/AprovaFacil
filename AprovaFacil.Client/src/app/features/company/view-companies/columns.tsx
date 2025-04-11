"use client"

import { Delete, MoreHorizontal, Settings } from "lucide-react"
import { Button } from "@/app/components/ui/button"
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from "@/app/components/ui/alert-dialog"
import { useNavigate } from "react-router-dom"
import { useIsMobile } from "@/hooks/use-mobile"
import { ColumnDef } from "@tanstack/react-table"
import { Popover, PopoverContent, PopoverTrigger } from "@/app/components/ui/popover"
import { Separator } from "@/app/components/ui/separator"
import { useRemoveCompanyMutation } from "@/app/api/companyApiSlice"
import { toast } from "@/hooks/use-toast";
import { CompanyResponse } from "@/types/company"
import { useIsAdmin } from "@/lib/utils"

const useColumns = () => {
  const navigate = useNavigate();
  const isMobile = useIsMobile();
  const isAdmin = useIsAdmin();
  const [removeCompany] = useRemoveCompanyMutation();

  const columns: ColumnDef<CompanyResponse>[] = [
    {
      accessorKey: "trade_name",
      header: "Nome",
      filterFn: "includesString",
      cell: ({ row }) => {
        const company = row.original;
        return (
          <div className="flex h-12 p-1 items-center">{company.trade_name}</div>
        )
      }
    },
    {
      accessorKey: "postal_code",
      header: "CEP",
    },
    {
      accessorKey: "state",
      header: "Estado",
      filterFn: "includesString",
    },
    {
      accessorKey: "city",
      header: "Cidade",
      filterFn: "includesString",
    },
    {
      accessorKey: "neighborhood",
      header: "Bairro",
      filterFn: "includesString",
    },
    {
      accessorKey: "street",
      header: "Rua",
      filterFn: "includesString",
    },
    {
      accessorKey: "phone",
      header: "Telefone",
      filterFn: "includesString",
      cell: ({ row }) => {
        const telefone = row.getValue<string>("phone");
        return (
          <div className="text-nowrap">
            <a href={`tel:${telefone}`}>{telefone}</a>
          </div>
        );
      }
    },
    {
      accessorKey: "email",
      header: "Email",
      filterFn: "includesString",
      cell: ({ row }) => {
        const email = row.getValue<string>("email");
        return (
          <div className="text-nowrap">
            <a href={`mailto:${email}`}>{email}</a>
          </div>
        );
      }
    },
    {
      id: "actions",
      header: "Editar",
      cell: ({ row }) => {
        const onDelete = async (company: CompanyResponse) => {
          try {
            await removeCompany(company.id);
          } catch (error) {
            console.error(error);

            if (error instanceof Error) {
              toast({
                title: "Falha ao remover empresa",
                description: `Não foi possível remover a empresa`
              });
            }
          }
        }

        const onEdit = (company: CompanyResponse) => {
          navigate(`/company/edit/${company.id}`);
        }

        const company = row.original;

        return (
          <div>
            <Popover>
              <PopoverTrigger asChild>
                <Button variant="outline" className="h-8 w-8 p-0">
                  <span className="sr-only">Open menu</span>
                  <MoreHorizontal className="h-4 w-4" />
                </Button>
              </PopoverTrigger>
              <PopoverContent side="left" className="w-fit p-1" sticky="always" >

                <div className="flex flex-col">
                  <Button variant={"ghost"} className="p-3 font-normal" onClick={() => onEdit(company)}>
                    <Settings /> Editar
                  </Button>

                  <Separator />

                  <AlertDialog>
                    <AlertDialogTrigger asChild>
                      <Button variant={"ghost"} className="p-3 font-normal" onSelect={(e) => e.preventDefault()}>
                        <Delete /> Remover
                      </Button>
                    </AlertDialogTrigger>
                    <AlertDialogContent>
                      <AlertDialogHeader>
                        <AlertDialogTitle>Você tem certeza ?</AlertDialogTitle>
                        <AlertDialogDescription>
                          <div className="flex flex-col  gap-2">
                            <span>Deseja prosseguir com a remoção da empresa '<strong>{company.trade_name}</strong>' ?</span>
                            <small>Obs.: As requisições realizadas em nome desta empresa <strong>não</strong> serão removidas.</small>
                          </div>
                        </AlertDialogDescription>
                      </AlertDialogHeader>
                      <AlertDialogFooter>
                        <AlertDialogCancel>Cancelar</AlertDialogCancel>
                        <AlertDialogAction onClick={() => onDelete(company)} >Continuar</AlertDialogAction>
                      </AlertDialogFooter>
                    </AlertDialogContent>
                  </AlertDialog>
                </div>

              </PopoverContent>
            </Popover>
          </div>
        )
      }
    }
  ];

  let _filteredColumns = [...columns];

  _filteredColumns =  !isAdmin 
  ? _filteredColumns.filter((col) => col.id !== "actions")
  : _filteredColumns;

  return isMobile
    ? _filteredColumns.filter((col) => !["postal_code", "state", "neighborhood", "street", "city"].includes(col.accessorKey))
    : _filteredColumns;
}

export default useColumns;
