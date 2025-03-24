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
import { useGetCurrentUserQuery } from "../api/authApiSlice"
import { RootState, useAppSelector } from "../store/store"

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

  const { data: authData } = useGetCurrentUserQuery();
  const { isAuthenticated } = useAppSelector((state: RootState) => state.auth);

  if (!isAuthenticated) {
    return <div></div>
  }
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
