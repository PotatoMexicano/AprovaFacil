"use client"

import { ArrowRightLeftIcon, MoreHorizontal, Settings } from "lucide-react"
import { Button } from "@/app/components/ui/button"
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from "@/app/components/ui/alert-dialog"
//import { useNavigate } from "react-router-dom"
import { useIsMobile } from "@/hooks/use-mobile"
import { ColumnDef } from "@tanstack/react-table"
import { Popover, PopoverContent, PopoverTrigger } from "@/app/components/ui/popover"
import { Separator } from "@/app/components/ui/separator"
//import { useRemoveCompanyMutation } from "@/app/api/companyApiSlice"
import { UserResponse } from "@/types/auth"
import { Badge } from "@/app/components/ui/badge"
import { cn, useIsAdmin } from "@/lib/utils"
import { useDisableUserMutation, useEnableUserMutation } from "@/app/api/userApiSlice"
import { RootState, useAppSelector } from "@/app/store/store"
import { useNavigate } from "react-router-dom"
import { toast } from "sonner"

interface Props {
  user: UserResponse,
  usuario: UserResponse
}

const ActionMenu = ({ user, usuario }: Props) => {
  const navigate = useNavigate();
  const [disableUser] = useDisableUserMutation();
  const [enableUser] = useEnableUserMutation();

  const onDisable = (usuario: UserResponse) => {
    disableUser(usuario.id).unwrap().catch(() => {
      toast.error("Falha ao desativar usuário");
    });
  }

  const onEnable = (usuario: UserResponse) => {
    enableUser(usuario.id).unwrap().catch(() => {
      toast.error("Falha ao desativar usuário");
    });
  }

  const onEdit = (usuario: UserResponse) => {
    navigate(`/users/edit/${usuario.id}`);
  }

  if (usuario.id === user.id) {
    return <>
      <div className="flex flex-col">
        <Button variant={"ghost"} className="p-3 font-normal" onClick={() => onEdit(usuario)}>
          <Settings /> Editar
        </Button>
      </div>
    </>
  }

  if (usuario.enabled) {
    return (
      <div className="flex flex-col">
        <Button variant={"ghost"} className="p-3 font-normal" onClick={() => onEdit(usuario)}>
          <Settings /> Editar
        </Button>
        <Separator />
        < AlertDialog >
          <AlertDialogTrigger asChild>
            <Button variant={"ghost"} className="p-3 font-normal" onSelect={(e) => e.preventDefault()}>
              <ArrowRightLeftIcon /> Desativar
            </Button>
          </AlertDialogTrigger>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Você tem certeza ?</AlertDialogTitle>
              <AlertDialogDescription>
                <div className="flex flex-col  gap-2">
                  <span>Deseja prosseguir com a desativação do usuário '<strong>{usuario.full_name}</strong>' ?</span>
                  <small>Obs.: As requisições realizadas em nome deste usuário <strong>não</strong> serão removidas.</small>
                </div>
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Cancelar</AlertDialogCancel>
              <AlertDialogAction onClick={() => onDisable(usuario)} >Continuar</AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog >
      </div>
    );
  } else {
    return (
      <div className="flex flex-col">
        <Button variant={"ghost"} className="p-3 font-normal" onClick={() => onEdit(usuario)}>
          <Settings /> Editar
        </Button>
        <Separator />
        <AlertDialog>
          <AlertDialogTrigger asChild>
            <Button variant={"ghost"} className="p-3 font-normal" onSelect={(e) => e.preventDefault()}>
              <ArrowRightLeftIcon /> Ativar
            </Button>
          </AlertDialogTrigger>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Você tem certeza ?</AlertDialogTitle>
              <AlertDialogDescription>
                <div className="flex flex-col  gap-2">
                  <span>Deseja prosseguir com a ativação do usuário '<strong>{usuario.full_name}</strong>' ?</span>
                </div>
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Cancelar</AlertDialogCancel>
              <AlertDialogAction onClick={() => onEnable(usuario)} >Continuar</AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </div>
    );
  }

}

const useColumns = () => {
  const isMobile = useIsMobile();
  const isAdmin = useIsAdmin();
  const { user } = useAppSelector((state: RootState) => state.auth);

  const columns: ColumnDef<UserResponse>[] = [
    {
      accessorKey: "picture_url",
      header: "Imagem",
      cell: ({ row }) => {

        const usuario = row.original

        return (
          <div>
            <img className={cn("h-12 p-1 border-2 m-1 rounded-full", usuario.enabled ? "border-green-500" : "border-red-400")} src={usuario.picture_url} alt={usuario.full_name} />
          </div>
        )
      }
    },
    {
      accessorKey: "full_name",
      header: "Nome",
      filterFn: "includesString"
    },
    {
      accessorKey: "email",
      header: "Email",
      filterFn: "includesString"
    },
    {
      accessorKey: "role_label",
      header: "Cargo",
      filterFn: "includesString"
    },
    {
      accessorKey: "department_label",
      header: "Setor",
      filterFn: "includesString"
    },
    {
      accessorKey: "enabled",
      header: "Ativo",
      cell: ({ row }) => {
        const usuario = row.original;

        return (
          (<Badge variant={usuario.enabled ? "success" : "destructive"} className="hover:text-white">{usuario.enabled ? "Ativo" : "Desativado"}</Badge>)
        )
      }
    },
    {
      id: "actions",
      header: "Editar",
      cell: ({ row }) => {
        const usuario = row.original
        if (usuario.id !== user?.id && !isAdmin) {
          return <></>
        }

        return (
          <div>
            <Popover>
              <PopoverTrigger asChild>
                <Button variant="outline" className="h-8 w-8 p-0">
                  <span className="sr-only bg-red-400">Open menu</span>
                  <MoreHorizontal className="h-4 w-4" />
                </Button>
              </PopoverTrigger>
              <PopoverContent side="left" className="w-fit p-1" sticky="always" >

                {user &&
                  <ActionMenu user={user} usuario={usuario} />
                }

              </PopoverContent>
            </Popover>
          </div>
        )
      }
    }
  ]

  return isMobile
    ? columns.filter((col) => !["enabled", "email"].includes(col.accessorKey))
    : columns;
}

export default useColumns;
