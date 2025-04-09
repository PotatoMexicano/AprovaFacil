import { useGetMyRequestsQuery, useGetMyStatsQuery } from "@/app/api/requestApiSlice";
import { Badge } from "@/app/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/app/components/ui/card";
import { useBreadcrumb } from "@/app/context/breadcrumb-context";
import { RootState, useAppSelector } from "@/app/store/store";
import { formatCurrency } from "@/lib/utils";
import { ArrowRight, BoxIcon, CheckCircle, ClockAlertIcon, FileText, Package2Icon, PackageCheckIcon, PackageIcon } from "lucide-react";
import { use, useEffect } from "react";
import { Link } from "react-router-dom";


export default function Homepage() {
  const { setBreadcrumbs } = useBreadcrumb();
  const { user } = useAppSelector((state: RootState) => state.auth);
  const { data: MyRecentsRequests, isLoading } = useGetMyRequestsQuery(5);
  const { data: MyStats } = useGetMyStatsQuery();

  useEffect(() => {
    setBreadcrumbs(["Início"]); // Define os breadcrumbs da página atual
  }, [setBreadcrumbs]);

  return (
    <div className="flex flex-1 flex-col gap-4 p-4 pt-0">
      <div className="w-full px-10 py-5 text-3xl font-semibold text-primary">
        Olá, {user?.full_name}! Que bom ter você por aqui. 🎉
      </div>

      <div className="grid auto-rows-min gap-4 md:grid-cols-3">

        {/* Total Requests */}
        <div className="rounded-xl" >
          <Card>
            <CardHeader>
              <div className="flex items-center space-x-4">
                <FileText className="h-6 w-6 text-primary" />
                <div className="w-full flex justify-between">
                  <CardTitle className="text-xl font-medium text-primary">
                    Total de Solicitações
                  </CardTitle>
                  <div className="text-2xl font-medium text-primary">
                    {MyStats?.total_requests}
                  </div>
                </div>
              </div>
            </CardHeader>
          </Card>
        </div>

        {/* Total Requests Pending */}
        <div className="rounded-xl">
          <Card>
            <CardHeader>
              <div className="flex items-center space-x-4">
                <ClockAlertIcon className="h-6 w-6 text-primary" />
                <div className="w-full flex justify-between">
                  <CardTitle className="text-xl font-medium text-primary">
                    Total de solicitações pendentes
                  </CardTitle>
                  <div className="text-2xl font-medium text-primary">
                    {MyStats?.total_request_pending}
                  </div>
                </div>
              </div>
            </CardHeader>
          </Card>
        </div>

        {/* Total amount approved */}
        <div className="rounded-xl">
          <Card>
            <CardHeader>
              <div className="flex items-center space-x-4">
                <ClockAlertIcon className="h-6 w-6 text-primary" />
                <div className="w-full flex justify-between">
                  <CardTitle className="text-xl font-medium text-primary">
                    Total movimentado
                  </CardTitle>
                  <div className="text-2xl font-medium text-primary">
                    {formatCurrency((MyStats?.total_amount_requests_approved ?? 0) / 100)}
                  </div>
                </div>
              </div>
            </CardHeader>
          </Card>
        </div>

      </div>
      
      {/* Recent Requests */}
      <div className="min-h-[100vh] flex-1 rounded-xl md:min-h-min">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Últimas Solicitações</CardTitle>
            <Link
              to="/request"
              className="flex items-center text-sm font-medium text-blue-600 hover:text-blue-700"
            >
              Ver todas
              <ArrowRight className="ml-1 h-4 w-4" />
            </Link>
          </CardHeader>
          <CardContent>
            <div className="divide-y divide-slate-200">
              {MyRecentsRequests && MyRecentsRequests.map((request) => (
                <div key={request.uuid} className="py-4">
                  <Link to={`/request/${request.uuid}`}>
                    <div className="flex items-center justify-between">
                      <div className="flex items-center">
                        <PackageIcon />
                        <div className="ml-4">
                          <p className="text-sm text-primary">{request.company.trade_name} - {formatCurrency(request.amount / 100)}</p>
                          <p className="text-sm text-primary/60">
                            Criado em {new Date(request.create_at).toLocaleDateString('pt-BR')}
                          </p>
                        </div>
                      </div>
                      <div>
                        <Badge variant={request.approved === 1
                          ? "success"
                          : request.approved === 0
                            ? "outline"
                            : "destructive"}>
                          {request.approved === 1
                            ? "Aprovado"
                            : request.approved === 0
                              ? "Pendente"
                              : "Recusado"}
                        </Badge>
                      </div>
                    </div>
                  </Link>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}