"use client"
import { useGetMyRequestsQuery, useGetMyStatsQuery, useGetTenantRequestsQuery, useGetTenantStatsQuery } from "@/app/api/requestApiSlice";
import { Badge } from "@/app/components/ui/badge";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card";
import { useBreadcrumb } from "@/app/context/breadcrumb-context";
import { RootState, useAppSelector } from "@/app/store/store";
import { formatCurrency, useIsAdmin } from "@/lib/utils";
import { ArrowRight, Building2Icon, ClockAlertIcon, FileText, LucideDollarSign, PackageIcon, User2Icon } from "lucide-react";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Line, LineChart, Pie, PieChart } from "recharts"
import { CartesianGrid, XAxis } from "recharts"
import { ChartConfig, ChartContainer, ChartLegend, ChartLegendContent, ChartTooltip, ChartTooltipContent } from "@/app/components/ui/chart"
import { Button } from "@/app/components/ui/button";

export default function Homepage() {
  const admin = useIsAdmin();
  const [stats, setStats] = useState(!admin);
  const { setBreadcrumbs } = useBreadcrumb();
  const { user } = useAppSelector((state: RootState) => state.auth);
  
  const { data: MyRecentsRequests } = useGetMyRequestsQuery(5);
  const {data: tenantRequests} = useGetTenantRequestsQuery(10);

  const dataRequests = stats ? MyRecentsRequests : tenantRequests;

  const { data: myStats } = useGetMyStatsQuery(undefined, { skip: !stats });
  const { data: tenantStats } = useGetTenantStatsQuery(undefined, { skip: stats });

  const dataStats = stats ? myStats : tenantStats;

  const chartConfig = {
    approved: {
      label: "Aprovado",
      color: "hsl(var(--chart-1))",
    },
    rejected: {
      label: "Recusado",
      color: "hsl(var(--chart-2))",
    },
    pending: {
      label: "Pendente",
      color: "hsl(var(--chart-3))",
    },
  } satisfies ChartConfig;

  const chartData = [
    { status: "approved", value: dataStats?.total_request_approved, fill: "hsl(var(--chart-1))" },
    { status: "rejected", value: dataStats?.total_request_rejected, fill: "hsl(var(--chart-2))" },
  ]

  const lineChartData = dataStats?.total_requests_by_month;

  useEffect(() => {
    setBreadcrumbs(["Início"]); // Define os breadcrumbs da página atual
  }, [setBreadcrumbs]);

  return (
    <div className="flex flex-1 flex-col gap-4 p-4 pt-0">
      <div className="w-full grid grid-cols-1 md:grid-cols-2 gap-4 px-0 py-5 text-3xl font-semibold text-foreground/90">
  <div>
    Olá, {user?.full_name}! Que bom ter você por aqui. 🎉
  </div>

  <div className="flex justify-end">
    {admin && (
      <Button onClick={() => setStats(!stats)} className="text-base">
        {stats ? (
          <>
            <p className="mr-2">Visualização: Empresa</p>
            <Building2Icon />
          </>
        ) : (
          <>
            <p className="mr-2">Visualização: Pessoal</p>
            <User2Icon />
          </>
        )}
      </Button>
    )}
  </div>
</div>


      <div className="grid auto-rows-min gap-4 md:grid-cols-3">

        {/* Total Requests */}
        <div className="rounded-xl" >
          <Card>
            <CardHeader>
              <div className="flex items-center space-x-4">
                <FileText className="h-6 w-6 text-foreground/90" />
                <div className="w-full flex justify-between">
                  <CardTitle className="text-xl font-medium text-foreground/90">
                    Total de Solicitações
                  </CardTitle>
                  <div className="text-2xl font-medium text-foreground/90">
                    {dataStats?.total_requests}
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
                <ClockAlertIcon className="h-6 w-6 text-foreground/90" />
                <div className="w-full flex justify-between">
                  <CardTitle className="text-xl font-medium text-foreground/90">
                    Total de solicitações pendentes
                  </CardTitle>
                  <div className="text-2xl font-medium text-foreground/90">
                    {dataStats?.total_request_pending}
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
                <LucideDollarSign className="h-6 w-6 text-foreground/90" />
                <div className="w-full flex justify-between">
                  <CardTitle className="text-xl font-medium text-foreground/90">
                    Total movimentado
                  </CardTitle>
                  <div className="text-2xl font-medium text-foreground/90">
                    {formatCurrency((dataStats?.total_amount_requests_approved ?? 0) / 100)}
                  </div>
                </div>
              </div>
            </CardHeader>
          </Card>
        </div>

      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">

        {/* Pie Chart - Proporção */}
        <Card>
          <CardHeader>
            <CardTitle>Proporção de solicitações</CardTitle>
            <CardDescription>
              Proporção de solicitações aprovadas para recusadas
            </CardDescription>
          </CardHeader>
          <CardContent className="flex-1 pb-0">
            <ChartContainer
              config={chartConfig}
              className="mx-auto aspect-square max-h-[300px]"
            >
              <PieChart>
                <ChartTooltip
                  cursor={false}
                  content={<ChartTooltipContent hideLabel />}
                />
                <Pie data={chartData} dataKey="value" nameKey="status" />
                <ChartLegend
                  content={<ChartLegendContent nameKey="status" />}
                  className="-translate-y-2 flex-wrap gap-2 [&>*]:basis-1/4 [&>*]:justify-center"
                />
              </PieChart>
            </ChartContainer>
          </CardContent>
        </Card>

        {/* Line Chart - Status mensal */}
        <Card>
          <CardHeader>
            <CardTitle>Status das Solicitações</CardTitle>
            <CardDescription>Últimos 12 meses</CardDescription>
          </CardHeader>
          <CardContent>
            <ChartContainer config={chartConfig}>
              <LineChart
                accessibilityLayer
                data={lineChartData}
                margin={{ left: 12, right: 12 }}
              >
                <CartesianGrid vertical={false} />
                <XAxis
                  dataKey="month"
                  tickLine={false}
                  axisLine={false}
                  tickMargin={8}
                />
                <ChartTooltip cursor={false} content={<ChartTooltipContent />} />
                <Line
                  dataKey="approved"
                  type="monotone"
                  stroke={chartConfig.approved.color}
                  strokeWidth={2}
                  dot={false}
                />
                <Line
                  dataKey="rejected"
                  type="monotone"
                  stroke={chartConfig.rejected.color}
                  strokeWidth={2}
                  dot={false}
                />
              </LineChart>
            </ChartContainer>
          </CardContent>
        </Card>

      </div>

      {/* Recent Requests */}
      <div className="min-h-[100vh] flex-1 rounded-xl md:min-h-min">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Últimas Solicitações</CardTitle>
            <Link
              to="/request"
              className="flex items-center text-sm font-medium text-primary hover:text-primary"
            >
              Ver todas
              <ArrowRight className="ml-1 h-4 w-4" />
            </Link>
          </CardHeader>
          <CardContent>
            <div className="divide-y divide-slate-200">
              {dataRequests && dataRequests.length > 0 ?
                dataRequests.map((request) => (
                  <div key={request.uuid} className="py-4">
                    <Link to={`/request/${request.uuid}`}>
                      <div className="flex items-center justify-between">
                        <div className="flex items-center">
                          <PackageIcon />
                          <div className="ml-4">
                            <p className="text-sm text-foreground/90">{request.company.trade_name} - {formatCurrency(request.amount / 100)}</p>
                            <p className="text-sm text-foreground/90/60">
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
                ))
                : (<div>Sem solicitações anteriores 😢</div>)}
            </div>
          </CardContent>
        </Card>
      </div>

    </div >
  )
}