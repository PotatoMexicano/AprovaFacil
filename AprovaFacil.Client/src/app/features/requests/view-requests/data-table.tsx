"use client"

import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
  ColumnFiltersState,
  getPaginationRowModel,
  getFilteredRowModel,
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
import { useState } from "react"
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/app/components/ui/select"
import { AnimatePresence, motion } from "framer-motion"
import { Search } from "lucide-react"
import { CompanyResponse } from "@/types/company"
import { RequestReponse } from "@/types/request"
import { UserResponse } from "@/types/auth"

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[]
  data: TData[]
}

export function DataTable<TData, TValue>({
  columns,
  data,
}: DataTableProps<TData, TValue>) {
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [globalFilter, setGlobalFilter] = useState("");
  const [selectedOption, setSelectedOption] = useState("10");
  const [pagination, setPagination] = useState({
    pageIndex: 0, //initial page index
    pageSize: Number(selectedOption), //default page size
  });

  const globalFilterFn = (row, columnId, filterValue) => {
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
    state: {
      globalFilter,
      pagination,
      columnFilters,
    },
  })

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

        <div className="flex ml-auto">
          <Select
            defaultValue="15"
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
                  No results.
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
