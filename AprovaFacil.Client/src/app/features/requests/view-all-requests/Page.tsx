"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Alert, AlertDescription, AlertTitle } from "@/app/components/ui/alert";
import { AlertCircle } from "lucide-react";
import { Skeleton } from "@/app/components/ui/skeleton";
import { useEffect, useState } from "react";
import { useBreadcrumb } from "@/app/context/breadkcrumb-context";
import { DataTable } from "../_shared/data-table";
import { useAllRequestsQuery, useGetMyRequestsQuery } from "@/app/api/requestApiSlice";
import { VisibilityState } from "@tanstack/react-table";
import { RootState, useAppSelector } from "@/app/store/store";

export default function ViewAllRequestsPage() {
  const { data, isLoading, error } = useAllRequestsQuery();
  const { setBreadcrumbs } = useBreadcrumb();

  const [columnVisibility, setColumnVisibility] = useState<VisibilityState>({
    "requester": true,
    "approved_first_level": false,
    "approved_first_level_at": false,
    "approved_second_level": false,
    "approved_second_level_at": false,
    "note": false,
    "received_at": false,
    "approved": true,
    "invoice_name": false,
    "budget_name": false,
    "actions": true,
  });

  useEffect(() => {
    setBreadcrumbs(["Início", "Solicitação", "Histórico"]); // Define os breadcrumbs da página atual
  }, [setBreadcrumbs]);

  return (
    <>
      <Card className="col-span-12 flex flex-col shadow-none border-0">
        <CardHeader>
          <CardTitle className="text-2xl w-full flex justify-between">
            Histórico de solicitações
          </CardTitle>
          <CardDescription>Histórico das solicitações registradas.</CardDescription>
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
                  <DataTable setColumnVisibility={setColumnVisibility} columnVisibility={columnVisibility} data={Array.isArray(data) ? data : []} />
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