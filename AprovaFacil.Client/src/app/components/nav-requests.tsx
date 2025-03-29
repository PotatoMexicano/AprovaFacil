"use client"

import { Boxes, PackageIcon, PackagePlus, type LucideIcon } from "lucide-react"

import {
  Collapsible,
} from "@/app/components/ui/collapsible"
import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/app/components/ui/sidebar"

export interface subItems {
  title: string
  url: string
  icon: LucideIcon
  isActive?: boolean
  isAccent?: boolean
  items?: {
    title: string
    url: string
  }[]
}

export function NavRequests() {
  return (
    <SidebarGroup>
      <SidebarGroupLabel>Requisições</SidebarGroupLabel>
      <SidebarMenu>

        <Collapsible asChild>
          <SidebarMenuItem>
            <SidebarMenuButton asChild tooltip="Nova solicitação">
              <a href="/request/register">
                <PackagePlus />
                <span>Nova solicitação</span>
              </a>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </Collapsible>

        <Collapsible asChild>
          <SidebarMenuItem>
            <SidebarMenuButton asChild tooltip="Minhas solicitações">
              <a href="/request/">
                <Boxes />
                <span>Minhas solicitações</span>
              </a>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </Collapsible>

        <Collapsible asChild>
          <SidebarMenuItem>
            <SidebarMenuButton asChild tooltip="Minhas solicitações">
              <a href="/request/">
                <PackageIcon />
                <span>Solicitações pendentes</span>
              </a>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </Collapsible>

      </SidebarMenu>
    </SidebarGroup>
  )
}
