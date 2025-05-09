import { ArrowDown, Building2, FileCheck2, LifeBuoyIcon, UserCheck2, Wallet } from "lucide-react";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Dialog, DialogContent, DialogDescription, DialogTitle, DialogTrigger } from "@/app/components/ui/dialog";
import { SidebarMenuButton } from "@/app/components/ui/sidebar";

export function WorkflowModal() {
  return (
    <Dialog>
      <DialogTrigger asChild>
        <SidebarMenuButton asChild tooltip="Nova solicitação">
          <div className="cursor-pointer">
            <LifeBuoyIcon />
            <span>Ajuda</span>
          </div>
        </SidebarMenuButton>
      </DialogTrigger>
      <DialogTitle></DialogTitle>
      <DialogContent className="w-[90vw] max-w-[600px] max-h-[90hv] overflow-y-auto bg-sidebar sm:rounded-xl sm:p-6">
        <DialogDescription></DialogDescription>
        <Card className="w-full border-none shadow-none bg-sidebar">
          <CardHeader>
            <CardTitle className="text-2xl">Fluxo de Atividades</CardTitle>
            <CardDescription>
              Entenda o processo de aprovação de solicitações no sistema
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col items-center space-y-4">
              {/* Cadastro de Empresa */}
              <div className="flex items-center justify-center bg-primary/10 p-4 rounded-lg w-64">
                <Building2 className="w-6 h-6 mr-2 text-primary" />
                <span className="font-medium">Cadastro de Empresa</span>
              </div>

              <ArrowDown className="w-6 h-6 text-muted-foreground" />

              {/* Nova Solicitação */}
              <div className="flex items-center justify-center bg-primary/10 p-4 rounded-lg w-64">
                <FileCheck2 className="w-6 h-6 mr-2 text-primary" />
                <span className="font-medium">Nova Solicitação</span>
              </div>

              <ArrowDown className="w-6 h-6 text-muted-foreground" />

              {/* Aprovação Gerente */}
              <div className="flex items-center justify-center bg-primary/10 p-4 rounded-lg w-64">
                <UserCheck2 className="w-6 h-6 mr-2 text-primary" />
                <span className="font-medium">Aprovação do Gerente</span>
              </div>

              <ArrowDown className="w-6 h-6 text-muted-foreground" />

              {/* Aprovação Diretor */}
              <div className="flex items-center justify-center bg-primary/10 p-4 rounded-lg w-64">
                <UserCheck2 className="w-6 h-6 mr-2 text-primary" />
                <span className="font-medium">Aprovação do Diretor</span>
              </div>

              <ArrowDown className="w-6 h-6 text-muted-foreground" />

              {/* Financeiro */}
              <div className="flex items-center justify-center bg-primary/10 p-4 rounded-lg w-64">
                <Wallet className="w-6 h-6 mr-2 text-primary" />
                <span className="font-medium">Financeiro</span>
              </div>
            </div>
          </CardContent>
          <CardFooter className="flex justify-between">
            <CardDescription className="text-sm text-muted-foreground">
              * Em caso de recusa em qualquer etapa, a solicitação é encerrada
            </CardDescription>
          </CardFooter>
        </Card>
      </DialogContent>
    </Dialog>
  );
}