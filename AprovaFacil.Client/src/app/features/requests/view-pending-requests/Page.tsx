"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Alert, AlertDescription, AlertTitle } from "@/app/components/ui/alert";
import { AlertCircle } from "lucide-react";
import { Skeleton } from "@/app/components/ui/skeleton";
import { useGetPendingRequestsQuery } from "@/app/api/requestApiSlice";
import { useEffect, useState } from "react";
import { useBreadcrumb } from "@/app/context/breadkcrumb-context";
import { DataTable } from "../_shared/data-table";
import { VisibilityState } from "@tanstack/react-table";
import { RootState, useAppSelector } from "@/app/store/store";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { useIsAdmin } from "@/lib/utils";

// Componente da tabela
export default function ViewPendingRequestsPage() {
  const navigate = useNavigate();
  const { user } = useAppSelector((state: RootState) => state.auth);
  
  const { data, isLoading, error } = useGetPendingRequestsQuery();
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
    setBreadcrumbs(["Início", "Solicitação", "Pendentes"]); // Define os breadcrumbs da página atual
  }, [setBreadcrumbs]);

  useEffect(() => {
    if (!user || !user.role) {
      toast.error("Você não possui permissão para acessar esse recurso.");
      console.error("Você não possui permissão para acessar esse recurso . #1")
      navigate("/")
      return;
    }

    if (!useIsAdmin){
      toast.error("Você não possui permissão para acessar esse recurso.");
      console.error("Você não possui permissão para acessar esse recurso . #2")
      navigate("/")
    }
  }, [user, navigate]);

  if (!user || !user.role) {
    return <div>Carregando...</div>; // Show a loading state while checking
  }

  return (
    <>
      <Card className="col-span-12 flex flex-col shadow-none border-0">
        <CardHeader>
          <CardTitle className="text-2xl w-full flex justify-between">
            Solicitação pendentes
          </CardTitle>
          <CardDescription>Solicitações pendentes para avaliação.</CardDescription>
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