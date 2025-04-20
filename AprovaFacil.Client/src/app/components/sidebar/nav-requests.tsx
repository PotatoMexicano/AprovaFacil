"use client"

import { Boxes, GalleryVerticalEndIcon, LucideCheckCheck, PackageIcon, PackagePlus, type LucideIcon } from "lucide-react"

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
import { RootState, useAppSelector } from "@/app/store/store"
import { useGetPendingRequestsQuery } from "@/app/api/requestApiSlice"
import { motion } from "framer-motion"

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
  const { data } = useGetPendingRequestsQuery();

  return (
    <>
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

        </SidebarMenu>
      </SidebarGroup>

      {user && (user.role === "Manager" || user.role === "Director")
        ? (
          <SidebarGroup>
            <SidebarGroupLabel>Administração</SidebarGroupLabel>
            <SidebarMenu>
              <Collapsible>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild tooltip="Solicitações pendentes">
                    <a href="/request/pending" className="relative">
                      <PackageIcon />
                      <span>Solicitações pendentes</span>
                      {data && data.filter(r => r.approved === 0).length > 0 && (
                        <motion.div
                          initial={{ opacity: 0 }}
                          animate={{ opacity: 1 }}
                          transition={{ duration: 0.5 }}
                        >
                          <div className="absolute right-2 w-2 h-2 aspect-auto rounded-full bg-orange-500"></div>
                        </motion.div>
                      )}
                    </a>
                  </SidebarMenuButton>
                </SidebarMenuItem>

                <SidebarMenuItem>
                  <SidebarMenuButton asChild tooltip="Todas as solicitações">
                    <a href="/request/all">
                      <GalleryVerticalEndIcon />
                      <span>Histórico de solicitações</span>
                    </a>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              </Collapsible>

            </SidebarMenu>
          </SidebarGroup>
        ) : null
      }

      {user && (user.role === "Finance")
        ? (
          <SidebarGroup>
            <SidebarGroupLabel>Financeiro</SidebarGroupLabel>
            <SidebarMenu>
              <Collapsible asChild>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild tooltip="Solicitações aprovadas">
                    <a href="/request/approved">
                      <LucideCheckCheck />
                      <span>Solicitações aprovadas</span>
                    </a>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              </Collapsible>
            </SidebarMenu>
          </SidebarGroup>
        )
        : null}
    </>
  )
}
