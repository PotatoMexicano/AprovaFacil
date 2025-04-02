import { useGetRequestQuery } from "@/app/api/requestApiSlice";
import { Card, CardContent, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Badge } from "@/app/components/ui/badge";
import { useParams } from "react-router-dom";
import {
  Building2,
  Calendar,
  Clock,
  DollarSign,
  FileText,
  ShieldCheck,
  User,
  UserCog,
} from 'lucide-react';
import { cn, formatCurrency, formatDate } from "@/lib/utils";
import { Avatar, AvatarFallback, AvatarImage } from "@/app/components/ui/avatar";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/app/components/ui/tooltip";
import { UserResponse } from "@/types/auth";

function UserCard({ user }: { user: UserResponse | undefined }) {

  if (!user)
    return null;

  return (
    <div className="flex items-center w-fit space-x-4 ">
      <TooltipProvider>
        <Tooltip>
          <TooltipTrigger asChild>
            <Avatar className={cn("size-12 p-1 border-2",
              user.request_approved === 1
                ? "border-green-400 bg-background"
                : user.request_approved === -1
                  ? "border-red-500 bg-background"
                  : "border-primary/20 bg-background"
            )}>
              <AvatarImage src={user.picture_url} />
              <AvatarFallback>{user.full_name}</AvatarFallback>
            </Avatar>
          </TooltipTrigger>
          <TooltipContent>
            <p>{user.full_name}</p>
          </TooltipContent>
        </Tooltip>
      </TooltipProvider>
      <div>
        <p className="text-sm font-medium">{user.full_name}</p>
        <p className="text-sm text-muted-foreground">{user.role_label}</p>
        <p className="text-xs text-muted-foreground">{user.department_label}</p>
      </div>
    </div>
  );
}

export default function ViewRequest() {
  const { id } = useParams<{ id: string }>();

  const { data, isLoading, isFetching, isError, error } = useGetRequestQuery(id as string);

  return (
    <div className="flex bg-background py-8">
      <div className="container mx-auto">
        <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
          {/* Amount Card */}
          <Card>
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="text-lg">Valor da nota</CardTitle>
                <DollarSign className="h-5 w-5 text-muted-foreground" />
              </div>
            </CardHeader>
            <CardContent>
              <p className="text-3xl font-bold">{data?.amount && formatCurrency(data?.amount / 100)}</p>
              <p className="text-sm text-muted-foreground">Pagamento para {data?.payment_date && formatDate(data?.payment_date)}</p>
            </CardContent>
          </Card>

          {/* Documents Card */}
          <Card>
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="text-lg">Documentos</CardTitle>
                <FileText className="h-5 w-5 text-muted-foreground" />
              </div>
            </CardHeader>
            <CardContent className="space-y-2">
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Nota Fiscal</span>
                <Badge variant={data?.has_invoice ? "success" : "destructive"}>{data?.has_invoice ? "Sim" : "Não"}</Badge>
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Orçamento</span>
                <Badge variant={data?.has_budget ? "success" : "destructive"}>{data?.has_budget ? "Sim" : "Não"}</Badge>
              </div>
            </CardContent>
          </Card>

          {/* Approval Status Card */}
          <Card>
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="text-lg">Aprovação</CardTitle>
                <Clock className="h-5 w-5 text-muted-foreground" />
              </div>
            </CardHeader>
            <CardContent className="space-y-2">
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Gerentes</span>
                {data?.approved === 1
                  ? (
                    <TooltipProvider>
                      <Tooltip>
                        <TooltipTrigger>
                          <Badge variant={"success"}>Sim</Badge>
                        </TooltipTrigger>
                        <TooltipContent>
                          {new Date(data?.first_level_at).toLocaleDateString("pt-BR")}
                        </TooltipContent>
                      </Tooltip>
                    </TooltipProvider>
                  )
                  : data?.approved === -1
                    ? <Badge variant={"destructive"}>Não</Badge>
                    : <Badge variant={"outline"}>Pendente</Badge>
                }
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Diretores</span>
                {data?.approved_second_level
                  ? (
                    <TooltipProvider>
                      <Tooltip>
                        <TooltipTrigger>
                          <Badge variant={"success"}>Sim</Badge>
                        </TooltipTrigger>
                        <TooltipContent>
                          {new Date(data?.second_level_at).toLocaleDateString("pt-BR")}
                        </TooltipContent>
                      </Tooltip>
                    </TooltipProvider>
                  )
                  : <Badge variant={"destructive"}>Não</Badge>
                }
              </div>
            </CardContent>
          </Card>

          {/* Timeline Card */}
          <Card>
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="text-lg">Linha do tempo</CardTitle>
                <Calendar className="h-5 w-5 text-muted-foreground" />
              </div>
            </CardHeader>
            <CardContent className="space-y-2">
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Created</span>
                <span className="text-sm font-medium">{data?.create_at ? formatDate(data.create_at) : ""}</span>
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Received</span>
                <span className="text-sm font-medium">{data?.received_at ? formatDate(data.received_at) : ""}</span>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Company Details */}
        <Card className="mt-6">
          <CardHeader>
            <div className="flex items-center justify-between">
              <CardTitle>Detalhes da Empresa</CardTitle>
              <Building2 className="h-5 w-5 text-muted-foreground" />
            </div>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <div>
                <p className="text-sm font-medium text-muted-foreground">Razão social</p>
                <p className="mt-1 text-sm">{data?.company.legal_name}</p>
              </div>
              <div>
                <p className="text-sm font-medium text-muted-foreground">Nome Fantasia</p>
                <p className="mt-1 text-sm">{data?.company.trade_name}</p>
              </div>
              <div>
                <p className="text-sm font-medium text-muted-foreground">CNPJ</p>
                <p className="mt-1 text-sm">{data?.company.cnpj}</p>
              </div>
              <div>
                <p className="text-sm font-medium text-muted-foreground">Contato</p>
                <p className="mt-1 text-sm">{data?.company.email}</p>
                <p className="text-sm">{data?.company.phone}</p>
              </div>
              <div className="sm:col-span-2">
                <p className="text-sm font-medium text-muted-foreground">Endereço</p>
                <p className="mt-1 text-sm">
                  {data?.company.street}, {data?.company.number}
                  {data?.company.complement && `, ${data?.company.complement}`}
                  <br />
                  {data?.company.neighborhood}, {data?.company.city}, {data?.company.state}
                  <br />
                  {data?.company.postal_code}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* People Section */}
        <div className="mt-6 grid grid-cols-1 gap-6 md:grid-cols-3">
          {/* Requester Card */}
          <Card>
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="text-lg">Solicitante</CardTitle>
                <User className="h-5 w-5 text-muted-foreground" />
              </div>
            </CardHeader>
            <CardContent>
              <UserCard user={data?.requester} />
            </CardContent>
          </Card>

          {/* Managers Card */}
          <Card>
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="text-lg">Gerentes</CardTitle>
                <UserCog className="h-5 w-5 text-muted-foreground" />
              </div>
            </CardHeader>
            <CardContent className="space-y-4">
              {data?.managers.map(manager => (
                <UserCard key={manager.id} user={manager} />
              ))}
            </CardContent>
          </Card>

          {/* Directors Card */}
          <Card>
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="text-lg">Gerentes</CardTitle>
                <UserCog className="h-5 w-5 text-muted-foreground" />
              </div>
            </CardHeader>
            <CardContent className="space-y-4">
              {data?.managers.map(manager => (
                <UserCard key={manager.id} user={manager} />
              ))}
            </CardContent>
          </Card>
        </div>

        {/* Notes */}
        {data?.note && (
          <Card className="mt-6">
            <CardHeader>
              <CardTitle>Notes</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-muted-foreground">{data?.note}</p>
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  );
}