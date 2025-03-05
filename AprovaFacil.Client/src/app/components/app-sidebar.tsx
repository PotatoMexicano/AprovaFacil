import * as React from "react"
import {
  Boxes,
  LifeBuoy,
  PackagePlus,
  PackageSearch,
  Building2
} from "lucide-react"

import { NavRequests } from "@/app/components/nav-requests"
import { NavCompanies } from "@/app/components/nav-companies"
import { NavSecondary } from "@/app/components/nav-secondary"
import { NavUser } from "@/app/components/nav-user"
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/app/components/ui/sidebar"

const data = {
  user: {
    name: "Jonh Doe",
    email: "jonh@doe.com",
    avatar: "/avatars/shadcn.jpg",
  },
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
  companies: [
    {
      name: "Empresa",
      url: "/company",
      icon: Building2,
    },
  ],
}

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
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
        <NavSecondary items={data.navSecondary} className="mt-auto" />
      </SidebarContent>
      <SidebarFooter>
        <NavUser user={data.user} />
      </SidebarFooter>
    </Sidebar>
  )
}
