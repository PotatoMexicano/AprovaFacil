import * as React from "react"
import {
  PackageSearch,
  Building2,
  UsersIcon,
} from "lucide-react"

import { NavRequests } from "@/app/components/sidebar/nav-requests"
import { NavCompanies } from "@/app/components/sidebar/nav-companies"
import { NavNotifications } from "@/app/components/sidebar/nav-notifications"
import { NavAuthUser } from "@/app/components/sidebar/nav-auth-user"
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
import { useGetCurrentUserQuery } from "@/app/api/authApiSlice"
import { RootState, useAppSelector } from "@/app/store/store"

const data = {
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

  const tenant_name = useAppSelector((state: RootState) => state.auth.user.tenant_name);
  const { data: authData } = useGetCurrentUserQuery();
    
  if (!authData) {
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
                  <span className="truncate text-xs">{tenant_name}</span>
                </div>
              </a>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarContent>
        <NavRequests />
        <NavCompanies projects={data.companies} />
        <NavUsers users={data.users} />
        <NavNotifications className="mt-auto" />
      </SidebarContent>
      <SidebarFooter>
        {authData && (
          <NavAuthUser user={authData} />
        )}
      </SidebarFooter>
    </Sidebar>
  )
}
