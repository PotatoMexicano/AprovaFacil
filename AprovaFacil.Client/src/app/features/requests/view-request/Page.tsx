import { useApproveRequestMutation, useGetRequestQuery, useLazyGetFileRequestQuery, useRejectRequestMutation } from "@/app/api/requestApiSlice";
import { Card, CardContent, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Badge } from "@/app/components/ui/badge";
import { useNavigate, useParams } from "react-router-dom";
import {
  AlertCircle,
  Building2,
  Calendar,
  CheckCircle,
  CircleCheckIcon,
  CircleXIcon,
  Clock,
  DollarSign,
  FileText,
  User,
  UserCog,
  XCircle,
} from 'lucide-react';
import { cn, formatCurrency, formatDate } from "@/lib/utils";
import { Avatar, AvatarFallback, AvatarImage } from "@/app/components/ui/avatar";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/app/components/ui/tooltip";
import { UserResponse } from "@/types/auth";
import { useEffect } from "react";
import { Button } from "@/app/components/ui/button";
import { useBreadcrumb } from "@/app/context/breadcrumb-context";
import { RootState, useAppSelector } from "@/app/store/store";
import { toast } from "sonner";
import { Alert, AlertDescription, AlertTitle } from "@/app/components/ui/alert";

function UserCard({ user }: { user: UserResponse | undefined }) {

  if (!user)
    return null;

  return (
    <div className="flex items-center w-fit space-x-4">
      <div className="relative group"> {/* Adiciona um contêiner relativo para posicionar o ícone */}
        <Avatar
          className={cn(
            "size-12 p-1 border-2 transition-opacity duration-300 group-hover:opacity-0", // Transição para ocultar a imagem no hover
            user.request_approved === 1
              ? "border-green-400 bg-background"
              : user.request_approved === -1
                ? "border-red-500 bg-background"
                : "border-primary/20 bg-background"
          )}
        >
          <AvatarImage src={user.picture_url} />
          <AvatarFallback>{user.full_name}</AvatarFallback>
        </Avatar>

        {/* Ícone de fundo que aparece no hover */}
        <div
          className={cn(
            "absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity duration-300", // Transição para mostrar o ícone
            user.request_approved === 1
              ? "text-green-400"
              : user.request_approved === -1
                ? "text-red-500"
                : "text-gray-400"
          )}
        >
          {user.request_approved === 1 && (
            <span className="text-2xl">✔</span> // Ícone de aprovado (check)
          )}
          {user.request_approved === -1 && (
            <span className="text-2xl">✖</span> // Ícone de recusado (X)
          )}
          {user.request_approved === 0 && (
            <span className="text-2xl">⏳</span> // Ícone de pendente (relógio)
          )}
        </div>
      </div>
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
  const { data, isLoading, isError, error, refetch } = useGetRequestQuery(id as string);
  const { user, isAuthenticated } = useAppSelector((state: RootState) => state.auth);
  const { setBreadcrumbs } = useBreadcrumb();
  const navigate = useNavigate();

  const toastId = "unauthorized-error";

  const [approveRequest] = useApproveRequestMutation();
  const [rejectRequest] = useRejectRequestMutation();

  useEffect(() => {
    setBreadcrumbs(["Início", "Solicitação", "Detalhes"]);
  }, [setBreadcrumbs])

  const [download, { data: fileData, error: fileError, isFetching: fileFetching }] = useLazyGetFileRequestQuery();

  const handleDownload = (fileType: string, requestId: string, fileId: string) => {
    download({
      fileId: fileId,
      fileType: fileType,
      requestId: requestId
    });
  };

  useEffect(() => {
    if (fileData && !fileFetching && !fileError) {
      const { blob, fileName } = fileData; // Desestrutura o resultado transformado
      const url = window.URL.createObjectURL(new Blob([blob], { type: 'application/pdf' }));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', fileName);
      document.body.appendChild(link);
      link.click();
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    }

    if (!fileFetching && fileError) {
      toast.error("Falha ao obter arquido PDF");
      console.error(fileError);
    }
  }, [fileData, fileFetching, fileError]);

  const handleReject = async () => {
    try {
      await rejectRequest(id as string).unwrap();
      refetch();
    }
    catch (err) {
      toast.error("Falha ao rejeitar solicitação.");
      console.error(err)
    }
  };
  const handleApprove = async () => {
    try {
      await approveRequest(id as string).unwrap();
      refetch();
    }
    catch (err) {
      toast.error("Falha ao aprovar solicitação.");
      console.error(err)
    }
  };

  const managerInRequest = data?.managers.find((manager) => manager.id === user?.id);
  const isManager = user?.role === "Manager" && managerInRequest !== undefined;

  const directorInRequest = data?.directors.find((director) => director.id === user?.id);
  const isDirector = user?.role === "Director" && directorInRequest !== undefined;

  useEffect(() => {
    if (error && !isLoading && isAuthenticated && error?.data?.status === 401) {
      const timer = setTimeout(() => {
        navigate("/request", { replace: true });
      }, 3500);
      toast.error("Sinto muito. Vamos te redirecionar...", {
        id: toastId
      });
      return () => clearTimeout(timer);
    }
  }, [isError, error, isLoading, navigate, isAuthenticated]);
  
  if (isLoading) {
    return <div>Carregando...</div>
  }
  
  if (isError && error) {
    return <>
      <Alert variant={"destructive"}>
        <AlertCircle className="w-4 h-4" />
        <AlertTitle>{error.data.title}</AlertTitle>
        <AlertDescription>{error.data.detail}</AlertDescription>
      </Alert>
    </>
  }

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
            <CardContent className="space-y-3">
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Nota Fiscal</span>
                {data && data.has_invoice
                  ? (
                    <TooltipProvider>
                      <Tooltip>
                        <TooltipTrigger>
                          <Button type="button" variant={"outline"} size={"sm"} onClick={() => handleDownload("invoice", data?.uuid, data?.invoice_name)}>
                            Baixar
                          </Button>
                        </TooltipTrigger>
                        <TooltipContent>
                          Baixar Nota Fiscal
                        </TooltipContent>
                      </Tooltip>
                    </TooltipProvider>
                  )
                  : (
                    <Badge variant={"destructive"} className="flex gap-1"><CircleXIcon size={16} /> Não</Badge>
                  )}
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Orçamento</span>
                {data && data.has_budget
                  ? (
                    <TooltipProvider>
                      <Tooltip>
                        <TooltipTrigger>
                          <Button type="button" variant={"outline"} size={"sm"} onClick={() => handleDownload("budget", data?.uuid, data?.budget_name)}>
                            Baixar
                          </Button>
                        </TooltipTrigger>
                        <TooltipContent>
                          Baixar Orçamento
                        </TooltipContent>
                      </Tooltip>
                    </TooltipProvider>
                  )
                  : (
                    <Badge variant={"destructive"} className="flex gap-1"><CircleXIcon size={16} /> Não</Badge>
                  )}
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
            <CardContent className="space-y-3">
              {data?.approved !== 0 && (
                <div className={`p-3 mb-4 text-center rounded-lg ${data?.approved === 1 ? 'bg-green-100' : 'bg-red-100'}`}>
                  <p className={`text-lg font-semibold ${data?.approved === 1 ? 'text-green-800' : 'text-red-800'}`}>
                    {data?.approved === 1 ? 'Aprovado' : 'Recusado'}
                  </p>
                </div>
              )}

              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Gerentes</span>
                {data?.approved_first_level === 1
                  ? (
                    <TooltipProvider>
                      <Tooltip>
                        <TooltipTrigger>
                          <Badge variant={"success"} className="flex gap-1"><CircleCheckIcon size={16} /> Aprovado</Badge>
                        </TooltipTrigger>
                        <TooltipContent>
                          {new Date(data?.first_level_at).toLocaleDateString("pt-BR")}
                        </TooltipContent>
                      </Tooltip>
                    </TooltipProvider>
                  )
                  : data?.approved_first_level === -1
                    ? <Badge variant={"destructive"} className="flex gap-1"><CircleXIcon size={16} /> Recusado</Badge>
                    : <Badge variant={"secondary"}>Pendente</Badge>
                }
              </div>

              {data && data?.directors.length > 0 && (
                <div className="flex items-center justify-between">
                  <span className="text-sm text-muted-foreground">Diretores</span>
                  {data?.approved_second_level === 1
                    ? (
                      <TooltipProvider>
                        <Tooltip>
                          <TooltipTrigger>
                            <Badge variant={"success"} className="flex gap-1"><CircleCheckIcon size={16} /> Aprovado</Badge>
                          </TooltipTrigger>
                          <TooltipContent>
                            {new Date(data?.second_level_at).toLocaleDateString("pt-BR")}
                          </TooltipContent>
                        </Tooltip>
                      </TooltipProvider>
                    )
                    : data?.approved_second_level === -1
                      ? <Badge variant={"destructive"} className="flex gap-1"><CircleXIcon size={16} /> Recusado</Badge>
                      : <Badge variant={"secondary"}>Pendente</Badge>
                  }
                </div>
              )}

              {isManager && managerInRequest && managerInRequest.request_approved === 0 && (
                <div className="flex gap-2 pt-2">
                  <button
                    onClick={handleReject}
                    className="flex-1 px-3 py-1.5 rounded text-sm flex items-center justify-center gap-1 text-red-600 border border-red-600 hover:bg-red-50 dark:hover:bg-red-800/30 transition-colors"
                  >
                    <XCircle className="w-4 h-4" />
                    Recusar
                  </button>
                  <button
                    onClick={handleApprove}
                    className="flex-1 px-3 py-1.5 rounded text-sm flex items-center justify-center gap-1 text-green-600 border border-green-600 hover:bg-green-50 dark:hover:bg-green-800/30 transition-colors"
                  >
                    <CheckCircle className="w-4 h-4" />
                    Aprovar
                  </button>
                </div>
              )}

              {isDirector && directorInRequest && directorInRequest.request_approved === 0 && data.level === 1 && (
                <div className="flex gap-2 pt-2">
                  <button
                    onClick={handleReject}
                    className="flex-1 px-3 py-1.5 rounded text-sm flex items-center justify-center gap-1 text-red-600 border border-red-600 hover:bg-red-50 dark:hover:bg-red-800/30 transition-colors"
                  >
                    <XCircle className="w-4 h-4" />
                    Recusar
                  </button>
                  <button
                    onClick={handleApprove}
                    className="flex-1 px-3 py-1.5 rounded text-sm flex items-center justify-center gap-1 text-green-600 border border-green-600 hover:bg-green-50 dark:hover:bg-green-800/30 transition-colors"
                  >
                    <CheckCircle className="w-4 h-4" />
                    Aprovar
                  </button>
                </div>
              )}
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
            <CardContent className="space-y-3">
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Aberto</span>
                <span className="text-sm font-normal">{data?.create_at ? formatDate(data.create_at) : ""}</span>
              </div>

              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Gerente</span>
                <span className="text-sm font-normal">{data?.first_level_at ? formatDate(data.first_level_at) : ""}</span>
              </div>

              {data && data.directors.length > 0 && (
                <div className="flex items-center justify-between">
                  <span className="text-sm text-muted-foreground">Diretor</span>
                  <span className="text-sm font-normal">{data?.second_level_at ? formatDate(data.second_level_at) : ""}</span>
                </div>
              )}

              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Faturado</span>
                {data && data.approved === 1 && (
                  <span className="text-sm font-medium">{data?.received_at ? formatDate(data.received_at) : ""}</span>
                )}
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
                <CardTitle className="text-lg">Gerente(s)</CardTitle>
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
          {data && data?.directors.length > 0 && (
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="text-lg">Diretor(s)</CardTitle>
                  <UserCog className="h-5 w-5 text-muted-foreground" />
                </div>
              </CardHeader>
              <CardContent className="space-y-4">
                {data?.directors.map(director => (
                  <UserCard key={director.id} user={director} />
                ))}
              </CardContent>
            </Card>
          )}
        </div>

        {/* Notes */}
        {data?.note && (
          <Card className="mt-6">
            <CardHeader>
              <CardTitle>Notas</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-muted-foreground whitespace-pre-line">{data?.note}</p>
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  );
}