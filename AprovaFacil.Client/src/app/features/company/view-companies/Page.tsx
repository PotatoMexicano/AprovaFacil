import { useBreadcrumb } from "@/app/context/breadcrumb-context"
import { useEffect } from "react";
import { DataTable } from "./data-table";
import { Skeleton } from "@/app/components/ui/skeleton";
import { useGetCompaniesQuery } from "@/app/api/companyApiSlice";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card";
import { useToast } from "@/hooks/use-toast";
import { Alert, AlertDescription, AlertTitle } from "@/app/components/ui/alert";
import { AlertCircle } from "lucide-react";
import useColumns from "./columns";
import { Outlet } from "react-router-dom";
import { Button } from "@/app/components/ui/button";

export default function ViewCompaniesPage() {
  const { toast } = useToast();
  const { setBreadcrumbs } = useBreadcrumb();

  const { data: companies, error: errorCompany, isError: isCompanyError, isLoading: isCompanyLoading } = useGetCompaniesQuery();
  const columns = useColumns();

  useEffect(() => {
    if (isCompanyError) {
      toast({
        title: "Falha ao obter dados das empresas",
        description: (errorCompany as Error).message
      })
    }
  }, [errorCompany, isCompanyError, toast]);

  useEffect(() => {
    setBreadcrumbs(["Início", "Empresa"]); // Define os breadcrumbs da página atual
  }, [setBreadcrumbs]);

  return (
    <>
      <Card className="col-span-12 flex flex-col shadow-none border-0">
        <CardHeader>
          <CardTitle className="text-2xl w-full flex justify-between">
            Empresas cadastradas

            <Button type="button" asChild><a href="/company/register">Adicionar empresa</a></Button>

          </CardTitle>
          <CardDescription>Empresas cadastradas para emissão de nota fiscal.</CardDescription>
        </CardHeader>
        <CardContent>
          {isCompanyLoading
            ? (
              <div className="flex flex-col space-y-3 py-4">
                <div className="space-y-2">
                  <Skeleton className="h-9 max-w-sm" />
                </div>
                <Skeleton className="h-[35rem] w-full rounded-xl" />
              </div>
            )
            : (
              !isCompanyError && companies
                ? (
                  <DataTable columns={columns} data={companies} />
                )
                : (
                  <Alert variant="destructive">
                    <AlertCircle className="h-4 w-4" />
                    <AlertTitle>Error</AlertTitle>
                    <AlertDescription>
                      {(errorCompany as Error).message}
                    </AlertDescription>
                  </Alert>
                )
            )}
        </CardContent>
      </Card>

      <Outlet />
    </>
  )
}