"use client";

import { ColumnDef, getCoreRowModel, RowExpanding, useReactTable, } from "@tanstack/react-table";
import { Button } from "@/app/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Alert, AlertDescription, AlertTitle } from "@/app/components/ui/alert";
import { AlertCircle } from "lucide-react";
import { Skeleton } from "@/app/components/ui/skeleton";
import { RequestReponse } from "@/types/request";
import { useGetMyRequestsQuery } from "@/app/api/requestApiSlice";
import { DataTable } from "../../requests/view-requests/data-table";
import { Avatar, AvatarFallback, AvatarImage } from "@/app/components/ui/avatar";
import { cn } from "@/lib/utils";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/app/components/ui/tooltip";

// Componente da tabela
export default function Example() {

  const { data, isLoading, error } = useGetMyRequestsQuery();

  const columns: ColumnDef<RequestReponse>[] = [
    {
      accessorKey: "requester",
      header: "Solicitante",
      cell: ({ row }) => {
        return (
          <div className="flex flex-col">
            <TooltipProvider>
              <Tooltip>
                <TooltipTrigger asChild>
                  <Avatar className={cn("size-12 p-1 border-2", row.original.requester.enabled
                    ? "border-green-400"
                    : "border-red-400"
                  )}>
                    <AvatarImage src={row.original.requester.picture_url} />
                    <AvatarFallback>{row.original.requester.full_name}</AvatarFallback>
                  </Avatar>
                </TooltipTrigger>
                <TooltipContent>
                  <p>{row.original.requester.full_name}</p>
                </TooltipContent>
              </Tooltip>
            </TooltipProvider>
          </div>
        )
      },
    },
    {
      accessorKey: "payment_date",
      header: "Data de Pagamento",
      cell: ({ row }) =>
        row.original.payment_date
          ? new Date(row.original.payment_date).toLocaleDateString("pt-BR")
          : "-",
    },
    {
      accessorKey: "create_at",
      header: "Data de Criação",
      cell: ({ row }) =>
        new Date(row.original.create_at).toLocaleDateString("pt-BR"),
    },
    {
      accessorKey: "approved_first_level_at",
      header: "Aprovado Nível 1",
      cell: ({ row }) =>
        row.original.approved_first_level_at
          ? new Date(row.original.approved_first_level_at).toLocaleDateString(
            "pt-BR"
          )
          : "-",
    },
    {
      accessorKey: "approved_second_level_at",
      header: "Aprovado Nível 2",
      cell: ({ row }) =>
        row.original.approved_second_level_at
          ? new Date(row.original.approved_second_level_at).toLocaleDateString(
            "pt-BR"
          )
          : "-",
    },
    {
      accessorKey: "received_at",
      header: "Data de Recebimento",
      cell: ({ row }) =>
        row.original.received_at
          ? new Date(row.original.received_at).toLocaleDateString("pt-BR")
          : "-",
    },
    {
      accessorKey: "amount",
      header: "Valor",
      cell: ({ row }) =>
        row.original.amount.toLocaleString("pt-BR", {
          style: "currency",
          currency: "BRL",
        }),
    },
    {
      accessorKey: "note",
      header: "Nota",
      cell: ({ row }) => row.original.note || "-",
    },
    {
      accessorKey: "managers",
      header: "Gerentes",
      cell: ({ row }) => (
        <div className="flex -space-x-1 overflow-hidden">
          {row.original.managers.length > 0 ? (
            row.original.managers.map((manager, index) => (
              <TooltipProvider key={index}>
                <Tooltip>
                  <TooltipTrigger asChild>
                    <Avatar
                      className={cn(
                        "size-12 p-1 border-2 rounded-full ring-2 ring-white bg-white",
                        manager.enabled ? "border-green-400" : "border-red-400"
                      )}
                    >
                      <AvatarImage src={manager.picture_url} />
                      <AvatarFallback>{manager.full_name}</AvatarFallback>
                    </Avatar>
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>{manager.full_name}</p>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            ))
          ) : (
            "-"
          )}
        </div>
      ),
    },
    {
      accessorKey: "directors",
      header: "Diretores",
      cell: ({ row }) => (
        <div className="flex -space-x-4 overflow-hidden">
          {row.original.directors.length > 0 ? (
            row.original.directors.map((director, index) => (
              <>
                <TooltipProvider key={index}>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <Avatar
                        className={cn(
                          "size-12 p-1 border-2 rounded-full ring-2 ring-white bg-white",
                          director.enabled ? "border-green-400" : "border-red-400"
                        )}
                      >
                        <AvatarImage src={director.picture_url} />
                        <AvatarFallback>{director.full_name}</AvatarFallback>
                      </Avatar>
                    </TooltipTrigger>
                    <TooltipContent>
                      <p>{director.full_name}</p>
                    </TooltipContent>
                  </Tooltip>
                </TooltipProvider>
              </>
            ))
          ) : (
            "-"
          )}
        </div>
      ),
    },
  ];

  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
  });

  return (
    <>
      <Card className="col-span-12 flex flex-col shadow-none border-0">
        <CardHeader>
          <CardTitle className="text-2xl w-full flex justify-between">
            Minhas solicitações
            <Button asChild>
              <a href="/request/register">Nova solicitação</a>
            </Button>
          </CardTitle>
          <CardDescription>Minhas requisições cadastradas.</CardDescription>
        </CardHeader>
        <CardContent>
          {isLoading
            ? (
              <div className="flex flex-col space-y-3 py-4">
                <div className="space-y-2">
                  <Skeleton className="h-9 max-w-sm" />
                </div>
                <Skeleton className="h-[35rem] w-full rounded-xl" />
              </div>
            )
            : (
              !error && data
                ? (
                  <DataTable columns={columns} data={Array.isArray(data) ? data : []} />
                )
                : (
                  <Alert variant="destructive">
                    <AlertCircle className="h-4 w-4" />
                    <AlertTitle>Error</AlertTitle>
                    <AlertDescription>
                      {error
                        ? 'status' in error
                          ? `Error ${error.status}: ${JSON.stringify(error.data)}`
                          : error.message
                        : "Unknown error"}
                    </AlertDescription>
                  </Alert>
                )
            )}
        </CardContent>
      </Card>
    </>
  );
}