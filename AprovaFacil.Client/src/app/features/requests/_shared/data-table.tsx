"use client"

import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
  ColumnFiltersState,
  getPaginationRowModel,
  getFilteredRowModel,
  VisibilityState,
  Row,
} from "@tanstack/react-table"

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/app/components/ui/table"

import { Button } from "@/app/components/ui/button"

import { Input } from "@/app/components/ui/input"
import { useEffect, useMemo, useState } from "react"
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/app/components/ui/select"
import { AnimatePresence, motion } from "framer-motion"
import { ArrowDownNarrowWideIcon, ExternalLinkIcon, Search } from "lucide-react"
import { RequestReponse } from "@/types/request"
import { UserResponse } from "@/types/auth"
import { DropdownMenu, DropdownMenuCheckboxItem, DropdownMenuContent, DropdownMenuTrigger } from "@/app/components/ui/dropdown-menu"
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/app/components/ui/tooltip"
import { Avatar, AvatarFallback, AvatarImage } from "@/app/components/ui/avatar"
import { cn } from "@/lib/utils"
import { Badge } from "@/app/components/ui/badge"
import { Link } from "react-router-dom"
import { FaFilePdf } from "react-icons/fa6"
import { useLazyGetFileRequestQuery } from "@/app/api/requestApiSlice"
import { toast } from "sonner"
import { RootState, useAppSelector } from "@/app/store/store"

interface DataTableProps<TData extends RequestReponse> {
  data: TData[];
  columnVisibility: VisibilityState;
  setColumnVisibility: React.Dispatch<React.SetStateAction<VisibilityState>>;
}

export function DataTable<TData extends RequestReponse>({ data, columnVisibility, setColumnVisibility }: DataTableProps<TData>) {
  const { user } = useAppSelector((state: RootState) => state.auth);
  const isManagerOrDirector = user.role === "Manager" || user.role === "Director";

  const handleDownload = (fileType: string, requestId: string, fileId: string) => {
    download({
      fileId: fileId,
      fileType: fileType,
      requestId: requestId
    });
  };

  const columns = useMemo(() => {
    if (!user) {
      return [];
    }

    const _columns: ColumnDef<RequestReponse>[] = [
      {
        accessorKey: "requester",
        header: "Solicitante",
        enableHiding: true,
        cell: ({ row }) => {
          return (
            <div className="flex flex-col">
              <TooltipProvider>
                <Tooltip>
                  <TooltipTrigger asChild>
                    <Avatar className={cn("size-12 p-1 border-2 border-primary/20 bg-background"
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
        accessorKey: "company",
        header: "Empresa",
        filterFn: "includesString",
        cell: ({ row }) => {
          return <>
            <TooltipProvider>
              <Tooltip>
                <TooltipTrigger asChild>
                  <Badge variant={"outline"}>
                    <Link to={`/company/edit/${row.original.company.id}`}>
                      {row.original.company.trade_name}
                    </Link>
                  </Badge>
                </TooltipTrigger>
                <TooltipContent>
                  {row.original.company.legal_name}
                </TooltipContent>
              </Tooltip>
            </TooltipProvider>
          </>
        }
      },
      {
        accessorKey: "create_at",
        header: "Data de Criação",
        cell: ({ row }) =>
          new Date(row.original.create_at).toLocaleDateString("pt-BR"),
      },
      {
        accessorKey: "approved_first_level",
        header: "Foi aprovado Nível 1",
        cell: ({ row }) =>
          <Badge variant={row.original.first_level_at
            ? "success"
            : "destructive"
          }>
            {row.original.approved_first_level
              ? "Sim"
              : "Não"}
          </Badge>
      },
      {
        accessorKey: "approved_first_level_at",
        header: "Aprovado Nível 1",
        cell: ({ row }) =>
          row.original.first_level_at
            ? new Date(row.original.first_level_at).toLocaleDateString(
              "pt-BR"
            )
            : "-",
      },
      {
        accessorKey: "approved_second_level",
        header: "Foi aprovado Nível 2",
        cell: ({ row }) =>
          <Badge variant={row.original.approved_second_level
            ? "success"
            : "destructive"
          }>
            {row.original.approved_second_level
              ? "Sim"
              : "Não"}
          </Badge>
      },
      {
        accessorKey: "approved_second_level_at",
        header: "Aprovado Nível 2",
        cell: ({ row }) =>
          row.original.second_level_at
            ? new Date(row.original.second_level_at).toLocaleDateString(
              "pt-BR"
            )
            : "-",
      },
      {
        accessorKey: "note",
        header: "Comentário",
      },
      {
        accessorKey: "received_at",
        header: "Data de Faturamento",
        cell: ({ row }) =>
          row.original.received_at
            ? new Date(row.original.received_at).toLocaleDateString("pt-BR")
            : "-",
      },
      {
        accessorKey: "amount",
        header: "Valor",
        cell: ({ row }) => {
          const currency = row.original.amount / 100;
          return currency.toLocaleString("pt-BR", {
            style: "currency",
            currency: "BRL",
          })
        },
      },
      {
        accessorKey: "approved",
        header: "Aprovado",
        cell: ({ row }) => {
          return <Badge variant={row.original.approved === 1
            ? "success"
            : row.original.approved === 0
              ? "outline"
              : "destructive"}>
            {row.original.approved === 1
              ? "Aprovado"
              : row.original.approved === 0
                ? "Pendente"
                : "Recusado"}
          </Badge>
        }
      },
      {
        accessorKey: "invoice_name",
        header: "Nota fiscal",
        cell: ({ row }) => {
          return <>
            <Button
              variant={"outline"}
              disabled={!row.original.has_invoice || fileFetching}
              size={"icon"}
              onClick={() => handleDownload("invoice", row.original.uuid, row.original.invoice_name)}>
              <FaFilePdf className="text-slate-700" />
            </Button>
          </>
        }
      },
      {
        accessorKey: "budget_name",
        header: "Orçamento",
        cell: ({ row }) => {
          return <>
            <Button
              variant={"outline"}
              disabled={!row.original.has_budget || fileFetching}
              size={"icon"}
              onClick={() => handleDownload("budget", row.original.uuid, row.original.budget_name)}>
              <FaFilePdf className="text-slate-700" />
            </Button>
          </>
        }
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
                      <Avatar className={cn("size-12 p-1 border-2",
                        manager.request_approved === 1
                        ? "border-green-400 bg-background"
                        : manager.request_approved === -1
                        ? "border-red-500 bg-background"
                        : "border-primary/20 bg-background"
                      )}>
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
                <TooltipProvider key={index}>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <Avatar className={cn("size-12 p-1 border-2",
                        director.request_approved === 1
                          ? "border-green-400 bg-background"
                          : director.request_approved === -1
                          ? "border-red-500 bg-background"
                          : "border-primary/20 bg-background"
                      )}>
                        <AvatarImage src={director.picture_url} />
                        <AvatarFallback>{director.full_name}</AvatarFallback>
                      </Avatar>
                    </TooltipTrigger>
                    <TooltipContent>
                      <p>{director.full_name}</p>
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
        accessorKey: "actions",
        header: "Info.",
        cell: ({ row }) => {
          return <TooltipProvider>
            <Tooltip>
              <TooltipTrigger asChild>
                <Button type="button" variant={"outline"} size={"icon"} asChild>
                  <Link to={`/request/${row.original.uuid}`} target="_blank" rel="noopener noreferrer" >
                    <ExternalLinkIcon />
                  </Link>
                </Button>
              </TooltipTrigger>
              <TooltipContent>
                Visualizar solicitação
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>
        }
      }
    ];

    if (isManagerOrDirector) {
      return _columns;
    }

    return _columns.filter((col) => 'accessorKey' in col && col.accessorKey !== "");
  }, [isManagerOrDirector, user]);

  const [download, { data: fileData, error: fileError, isFetching: fileFetching }] = useLazyGetFileRequestQuery();
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [globalFilter, setGlobalFilter] = useState("");
  const [selectedOption, setSelectedOption] = useState("10");

  const [pagination, setPagination] = useState({
    pageIndex: 0, //initial page index
    pageSize: Number(selectedOption), //default page size
  });

  const globalFilterFn = (row: Row<RequestReponse>, columnId: string, filterValue: string) => {
    if (!filterValue) return true;

    const value = filterValue.toLowerCase();

    // Acessa o objeto company dentro de row.original
    const request: RequestReponse = row.original || {};

    // Converte os valores para string e lowercase, tratando casos undefined/null
    const tradeName = String(request.company.trade_name || "").toLowerCase();
    const legalName = String(request.company.legal_name || "").toLowerCase();
    const requester = String(request.requester.full_name || "").toLowerCase();
    const create_at = String(request.create_at || "").toLowerCase();
    const amount = String(request.amount || "").toLowerCase();
    const approved = String(request.approved || "").toLowerCase();

    const directors: UserResponse[] = Array.isArray(row.original.directors) ? row.original.directors : [];
    const managers: UserResponse[] = Array.isArray(row.original.managers) ? row.original.managers : [];

    // Verifica arrays
    const hasMatchingDirector = directors.some(director =>
      String(director?.full_name || "").toLowerCase().includes(value)
    );
    const hasMatchingManager = managers.some(manager =>
      String(manager?.full_name || "").toLowerCase().includes(value)
    );

    // Verifica se o valor do filtro está presente em qualquer um dos campos
    return (
      tradeName.includes(value) ||
      legalName.includes(value) ||
      requester.includes(value) ||
      create_at.includes(value) ||
      amount.includes(value) ||
      approved.includes(value) ||
      hasMatchingDirector ||
      hasMatchingManager
    );
  };

  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
    onColumnFiltersChange: setColumnFilters,
    getPaginationRowModel: getPaginationRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    onPaginationChange: setPagination,
    onGlobalFilterChange: setGlobalFilter,
    globalFilterFn: globalFilterFn,
    onColumnVisibilityChange: setColumnVisibility,
    state: {
      globalFilter,
      pagination,
      columnFilters,
      columnVisibility,
    },
  })

  useEffect(() => {
    if (fileData && !fileFetching && !fileError) {
      const { blob, fileName } = fileData; // Desestrutura o resultado transformado
      const url = window.URL.createObjectURL(new Blob([blob], { type: 'application/pdf' }));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', fileName);
      document.body.appendChild(link);
      link.click();
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    }

    if (!fileFetching && fileError) {
      toast.error("Falha ao obter arquido PDF");
      console.error(fileError);
    }
  }, [fileData, fileFetching, fileError]);

  return (
    <div>
      <div className="flex items-center py-4 gap-4">
        <div className="relative w-96">
          <Input
            placeholder="Filtrar por nome, data, valor ou situação..."
            value={globalFilter ?? ""}
            onChange={(event) => setGlobalFilter(event.target.value)}
            className="pr-8"
          />
          {globalFilter
            ? (
              <button
                onClick={() => setGlobalFilter("")}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            )
            : (
              <div className="absolute right-3 top-1/2 -translate-y-1/2 h-4 w-4">
                <AnimatePresence mode="popLayout">
                  <motion.div
                    key="search"
                    initial={{ y: -20, opacity: 0 }}
                    animate={{ y: 0, opacity: 1 }}
                    exit={{ y: 20, opacity: 0 }}
                    transition={{ duration: 0.2 }}
                  >
                    <Search className="w-4 h-4 text-gray-400 dark:text-gray-500" />
                  </motion.div>
                </AnimatePresence>
              </div>
            )}
        </div>

        <div className="flex ml-auto gap-4">
          <Select
            defaultValue="5"
            value={selectedOption}
            onValueChange={(value) => {
              setSelectedOption(value);
              setPagination(() => ({
                pageIndex: 0,
                pageSize: Number(value),
              }));
            }}>
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="Quantidade registros" />
            </SelectTrigger>
            <SelectContent>
              <SelectGroup>
                <SelectLabel>Items p/ página</SelectLabel>
                <SelectItem value="5">5 items</SelectItem>
                <SelectItem value="10">10 items</SelectItem>
                <SelectItem value="15">15 items</SelectItem>
                <SelectItem value="30">30 items</SelectItem>
                <SelectItem value="50">50 items</SelectItem>
              </SelectGroup>
            </SelectContent>
          </Select>

          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="outline">
                <ArrowDownNarrowWideIcon />
                Colunas
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              {table
                .getAllColumns()
                .filter(
                  (column) => column.getCanHide()
                )
                .map((column) => {
                  return (
                    <DropdownMenuCheckboxItem
                      key={column.id}
                      className="capitalize"
                      checked={column.getIsVisible()}
                      onCheckedChange={(value) =>
                        column.toggleVisibility(!!value)
                      }
                    >
                      {column.id}
                    </DropdownMenuCheckboxItem>
                  )
                })}
            </DropdownMenuContent>
          </DropdownMenu>

        </div>

      </div>
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => {
                  return (
                    <TableHead key={header.id}>
                      {header.isPlaceholder
                        ? null
                        : flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                    </TableHead>
                  )
                })}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow
                  key={row.id}
                  data-state={row.getIsSelected() && "selected"}
                >
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={columns.length} className="h-24 text-center">
                  Nenhum resultado.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
      <div className="flex items-center justify-end space-x-2 py-4">
        <Button
          variant="outline"
          size="sm"
          onClick={() => table.previousPage()}
          disabled={!table.getCanPreviousPage()}
        >
          Anterior
        </Button>
        <Button
          variant="outline"
          size="sm"
          onClick={() => table.nextPage()}
          disabled={!table.getCanNextPage()}
        >
          Próximo
        </Button>
      </div>
    </div>
  )
}
