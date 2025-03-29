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
import React from "react"
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/app/components/ui/select"
import { AnimatePresence, motion } from "framer-motion"
import { Search } from "lucide-react"

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
    const trade_name = String(row.getValue("trade_name") || "").toLowerCase();
    const state = String(row.getValue("state") || "").toLowerCase();
    const city = String(row.getValue("city") || "").toLowerCase();
    const neighborhood = String(row.getValue("neighborhood") || "").toLowerCase();
    const street = String(row.getValue("street") || "").toLowerCase();
    const phone = String(row.getValue("phone") || "").toLowerCase();
    const email = String(row.getValue("email") || "").toLowerCase();

    return (
      trade_name.includes(value) ||
      state.includes(value) ||
      city.includes(value) ||
      neighborhood.includes(value) ||
      email.includes(value) ||
      street.includes(value) ||
      phone.includes(value)
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
    globalFilterFn,
    onGlobalFilterChange: setGlobalFilter,
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
            placeholder="Filtrar por nome, email, cargo ou setor..."
            value={globalFilter ?? ""}
            onChange={(event) => setGlobalFilter(event.target.value)}
            className="pr-8"
          />
          {globalFilter && (
            <button
              onClick={() => setGlobalFilter("")}
              className="absolute right-2 top-1/2 -translate-y-1/2 text-gray-500 hover:text-gray-700"
            >
              ✕
            </button>
          )}

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
