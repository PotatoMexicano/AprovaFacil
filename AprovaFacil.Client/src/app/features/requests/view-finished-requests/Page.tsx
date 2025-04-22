"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Alert, AlertDescription, AlertTitle } from "@/app/components/ui/alert";
import { AlertCircle, CheckCheck } from "lucide-react";
import { Skeleton } from "@/app/components/ui/skeleton";
import { useGetApprovedRequestsQuery, useGetFinishedRequestsQuery, useGetPendingRequestsQuery } from "@/app/api/requestApiSlice";
import { useEffect, useState } from "react";
import { useBreadcrumb } from "@/app/context/breadcrumb-context";
import { DataTable } from "../_shared/data-table";
import { VisibilityState } from "@tanstack/react-table";
import { RootState, useAppSelector } from "@/app/store/store";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { useIsAdmin, useIsFinance } from "@/lib/utils";

// Componente da tabela
export default function ViewFinishedRequestsPage() {
  const navigate = useNavigate();
  const isFinance = useIsFinance();
  const { user } = useAppSelector((state: RootState) => state.auth);
  const toastId = "unauthorized-error";
  const { data, isLoading, error } = useGetFinishedRequestsQuery();
  const { setBreadcrumbs } = useBreadcrumb();

  const [columnVisibility, setColumnVisibility] = useState<VisibilityState>({
    "requester": true,
    "approved_first_level": false,
    "approved_second_level": false,
    "note": false,
    "received_at": false,
    "approved": true,
    "invoice_name": true,
    "budget_name": true
  });

  useEffect(() => {
    setBreadcrumbs(["Início", "Solicitação", "Faturadas"]); // Define os breadcrumbs da página atual
  }, [setBreadcrumbs]);

  useEffect(() => {
    if (!isFinance) {
      toast.error("Você não possui permissão para acessar esse recurso.", {
        id: toastId,
        duration: 3000,
      });
      console.error("Você não possui permissão para acessar esse recurso . #2")

      const timer = setTimeout(() => {
        navigate("/", { replace: true });
      }, 3000);
  
      return () => clearTimeout(timer);
    }
  }, [isFinance, navigate]);

  if (!user || !user.role) {
    return <div>Carregando...</div>; // Show a loading state while checking
  }

  return (
    <>
      <Card className="col-span-12 flex flex-col shadow-none border-0">
        <CardHeader>
          <CardTitle className="text-2xl w-full flex justify-between">
            Solicitação faturadas
          </CardTitle>
          <CardDescription>Solicitações faturadas. </CardDescription>
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