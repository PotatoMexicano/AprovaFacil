"use client"

import { Delete, MoreHorizontal, Settings } from "lucide-react"
import { Button } from "@/app/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/app/components/ui/dropdown-menu"
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from "@/app/components/ui/alert-dialog"
import { useNavigate } from "react-router-dom"
import { useIsMobile } from "@/hooks/use-mobile"
import { ColumnDef } from "@tanstack/react-table"
import { Popover, PopoverContent, PopoverTrigger } from "@/app/components/ui/popover"
import { Separator } from "@/app/components/ui/separator"
import { useRemoveCompanyMutation } from "@/app/api/companyApiSlice"
import { toast } from "@/hooks/use-toast";
import { Company } from "@/types/company"

const useColumns = () => {
  const navigate = useNavigate();
  const isMobile = useIsMobile();
  const [removeCompany] = useRemoveCompanyMutation();

  const columns: ColumnDef<Company>[] = [
    {
      accessorKey: "trade_name",
      header: "Nome",
    },
    {
      accessorKey: "postal_code",
      header: "CEP",
    },
    {
      accessorKey: "state",
      header: "Estado",
    },
    {
      accessorKey: "city",
      header: "Cidade",
    },
    {
      accessorKey: "neighborhood",
      header: "Bairro",
    },
    {
      accessorKey: "street",
      header: "Rua",
    },
    {
      accessorKey: "phone",
      header: "Telefone",
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
      cell: ({ row }) => {

        const onDelete = async (company: Company) => {
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

        const onEdit = (company: Company) => {
          navigate(`/company/edit/${company.id}`);
        }

        const company = row.original

        return (

          isMobile ? (
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
                    <Button variant={"ghost"} className="p-3" onClick={() => onEdit(company)}>
                      <Settings /> Editar
                    </Button>

                    <Separator />

                    <AlertDialog>
                      <AlertDialogTrigger asChild>
                        <Button variant={"ghost"} className="p-3" onSelect={(e) => e.preventDefault()}>
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
            : (
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant="outline" className="h-8 w-8 p-0">
                    <span className="sr-only">Open menu</span>
                    <MoreHorizontal className="h-4 w-4" />
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="center" side="left" >

                  <DropdownMenuItem onClick={() => onEdit(company)}>
                    <Settings /> Editar
                  </DropdownMenuItem>

                  <DropdownMenuSeparator />

                  <AlertDialog>
                    <AlertDialogTrigger asChild>
                      <DropdownMenuItem onSelect={(e) => e.preventDefault()}>
                        <Delete /> Remover
                      </DropdownMenuItem>
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

                </DropdownMenuContent>
              </DropdownMenu>
            )
        )
      }
    }
  ]

  return isMobile
    ? columns.filter((col) => !["postal_code", "state", "neighborhood", "street", "city"].includes(col.accessorKey))
    : columns;
}

export default useColumns;
