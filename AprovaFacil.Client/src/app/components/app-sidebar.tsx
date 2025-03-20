import * as React from "react"
import {
  Boxes,
  LifeBuoy,
  PackagePlus,
  PackageSearch,
  Building2,
  UsersIcon
} from "lucide-react"

import { NavRequests } from "@/app/components/nav-requests"
import { NavCompanies } from "@/app/components/nav-companies"
import { NavSecondary } from "@/app/components/nav-secondary"
import { NavAuthUser } from "@/app/components/nav-auth-user"
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/app/components/ui/sidebar"
import { NavUsers } from "./nav-users"
import { useGetCurrentUserQuery, useLogoutMutation } from "../api/authApiSlice"
import { toast } from "@/hooks/use-toast"
import { useNavigate } from "react-router-dom"
import { useDispatch } from "react-redux"
import { clearUser } from "@/auth/authSlice"

const data = {
  navMain: [
    {
      title: "Nova solicitação",
      url: "/request/register",
      icon: PackagePlus,
      isAccent: true,
    },
    {
      title: "Minhas solicitações",
      url: "/request/",
      icon: Boxes,
    },
  ],
  navSecondary: [
    {
      title: "Ajuda",
      url: "#",
      icon: LifeBuoy,
    }
  ],
  users: [
    {
      name: "Usuários",
      url: "/users",
      icon: UsersIcon,
    }
  ],
  companies: [
    {
      name: "Empresa",
      url: "/company",
      icon: Building2,
    },
  ],
}

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {

  const navigate = useNavigate();
  const dispatch = useDispatch();

  const { data: authData , error} = useGetCurrentUserQuery();
  const [logout] = useLogoutMutation();

  React.useEffect(() => {
    if (error && 'status' in error && error.status === 401) {
      logout()
      .unwrap()
      .then(() => {
        
        dispatch(clearUser());

        toast({
          title: "Sessão Expirada",
          description: "Você foi desconectado. Faça login novamente.",
          variant: "destructive",
        });
        navigate("/login", {replace: true}); // Use navigate instead of window.location.href
      })
      .catch((logoutError) => {
        console.error("Erro ao fazer logout:", logoutError);
        toast({
          title: "Erro",
          description: "Falha ao fazer logout. Tente novamente.",
          variant: "destructive",
        });
      });
    }
  }, [error])
  
  return (
    <Sidebar variant="inset" {...props}>
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg" asChild>
              <a href="/">
                <div className="flex aspect-square size-8 items-center justify-center rounded-lg bg-sidebar-primary text-sidebar-primary-foreground">
                  <PackageSearch className="size-4" />
                </div>
                <div className="grid flex-1 text-left text-sm leading-tight">
                  <span className="truncate font-semibold">AprovaFácil</span>
                  <span className="truncate text-xs">Embraplan</span>
                </div>
              </a>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarContent>
        <NavRequests items={data.navMain} />
        <NavCompanies projects={data.companies} />
        <NavUsers users={data.users} />
        <NavSecondary items={data.navSecondary} className="mt-auto" />
      </SidebarContent>
      <SidebarFooter>
        {authData && (
          <NavAuthUser user={authData} />
        )}
      </SidebarFooter>
    </Sidebar>
  )
}
