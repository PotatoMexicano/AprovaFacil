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
import { RootState, useAppSelector } from "../store/store"

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
  const { user } = useAppSelector((state: RootState) => state.auth);

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

        {user && (user.role === "Manager" || user.role === "Director")
        ? (
          <Collapsible asChild>
            <SidebarMenuItem>
              <SidebarMenuButton asChild tooltip="Minhas solicitações">
                <a href="/request/pending">
                  <PackageIcon />
                  <span>Solicitações pendentes</span>
                </a>
              </SidebarMenuButton>
            </SidebarMenuItem>
          </Collapsible>
        )
      : null}

      </SidebarMenu>
    </SidebarGroup>
  )
}
