import { useBreadcrumb } from "@/app/context/breadkcrumb-context"
import { useEffect } from "react";
import { DataTable } from "./data-table";
import { Skeleton } from "@/app/components/ui/skeleton";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card";
import { useToast } from "@/hooks/use-toast";
import { Alert, AlertDescription, AlertTitle } from "@/app/components/ui/alert";
import { AlertCircle } from "lucide-react";
import useColumns from "./columns";
import { Outlet } from "react-router-dom";
import { Button } from "@/app/components/ui/button";
import { useGetUsersQuery } from "@/app/api/userApiSlice";

export default function ViewUsersPage() {
  const { toast } = useToast();
  const { setBreadcrumbs } = useBreadcrumb();

  const {data: users, error: errorUsers, isError: isErrorUsers, isLoading: isLoadingUsers} = useGetUsersQuery();
  const columns = useColumns();

  useEffect(() => {
    if (isErrorUsers) {
      toast({
        title: "Falha ao obter dados dos usuários",
        description: (errorUsers as Error).message
      })
    }
  }, [errorUsers, isErrorUsers, toast]);

  useEffect(() => {
    setBreadcrumbs(["Início", "Usuários"]); // Define os breadcrumbs da página atual
  }, [setBreadcrumbs]);

  return (
    <>
      <Card className="col-span-12 flex flex-col shadow-none border-0">
        <CardHeader>
          <CardTitle className="text-2xl w-full flex justify-between">
            Usuários cadastradas
            <Button asChild><a href="/users/register">Adicionar usuário</a></Button>
            
          </CardTitle>
          <CardDescription>Usuários cadastrados no sistema.</CardDescription>
        </CardHeader>
        <CardContent>
          {isLoadingUsers
            ? (
              <div className="flex flex-col space-y-3 py-4">
                <div className="space-y-2">
                  <Skeleton className="h-9 max-w-sm" />
                </div>
                <Skeleton className="h-[35rem] w-full rounded-xl" />
              </div>
            )
            : (
              !isErrorUsers && users
                ? (
                  <DataTable columns={columns} data={users} />
                )
                : (
                  <Alert variant="destructive">
                    <AlertCircle className="h-4 w-4" />
                    <AlertTitle>Error</AlertTitle>
                    <AlertDescription>
                      {(errorUsers as Error).message}
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