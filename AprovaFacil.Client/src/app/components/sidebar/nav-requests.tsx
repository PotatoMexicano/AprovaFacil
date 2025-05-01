"use client"

import { BanknoteIcon, Boxes, BoxesIcon, GalleryVerticalEndIcon, LucideCheckCheck, PackageIcon, PackageOpenIcon, PackagePlus, type LucideIcon } from "lucide-react"

import {
  Collapsible,
} from "@/app/components/ui/collapsible"
import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuBadge,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/app/components/ui/sidebar"
import { RootState, useAppSelector } from "@/app/store/store"
import { useLazyGetApprovedRequestsQuery, useLazyGetPendingRequestsQuery } from "@/app/api/requestApiSlice"
import { motion } from "framer-motion"
import { useEffect } from "react"
import { useIsAdmin, useIsFinance } from "@/lib/utils"

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
  const isAdmin = useIsAdmin();
  const isFinance = useIsFinance();

  const { user } = useAppSelector((state: RootState) => state.auth);

  const [triggerPending, { data: dataPending }] = useLazyGetPendingRequestsQuery();
  const [triggerApproved, { data: dataApproved }] = useLazyGetApprovedRequestsQuery();

  useEffect(() => {
    if (isAdmin) {
      triggerPending(); // dispara a query somente uma vez ao montar
    }

    if (isFinance) {
      triggerApproved();
    }
  }, []);

  return (
    <>
      <SidebarGroup>
        <SidebarGroupLabel>Requisições</SidebarGroupLabel>
        <SidebarMenu>

          <Collapsible asChild>
            <SidebarMenuItem>
              <SidebarMenuButton asChild tooltip="Nova solicitação">
                <a href="/request/register">
                  <PackagePlus strokeWidth={1.75} absoluteStrokeWidth />
                  <span>Nova solicitação</span>
                </a>
              </SidebarMenuButton>
            </SidebarMenuItem>
          </Collapsible>

          <Collapsible asChild>
            <SidebarMenuItem>
              <SidebarMenuButton asChild tooltip="Minhas solicitações">
                <a href="/request/">
                  <PackageOpenIcon strokeWidth={1.75} absoluteStrokeWidth />
                  <span>Minhas solicitações</span>
                </a>
              </SidebarMenuButton>
            </SidebarMenuItem>
          </Collapsible>

        </SidebarMenu>
      </SidebarGroup>

      {user && isAdmin
        ? (
          <SidebarGroup>
            <SidebarGroupLabel>Administração</SidebarGroupLabel>
            <SidebarMenu>
              <Collapsible>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild tooltip="Solicitações pendentes">
                    <a href="/request/pending" className="relative text-nowrap">
                      <PackageIcon strokeWidth={1.75} absoluteStrokeWidth />
                      <span >Solicitações pendentes</span>
                      {dataPending && dataPending.filter(r => r.approved === 0).length > 0 && (
                        <motion.div
                          className="w-full h-full flex items-center"
                          initial={{ opacity: 0 }}
                          animate={{ opacity: 1 }}
                          transition={{ duration: 0.5 }}
                        >
                          <SidebarMenuBadge>
                            <div className="absolute right-2 w-2 h-2 aspect-auto rounded-full bg-success-light" />
                          </SidebarMenuBadge>
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
        ) : <></>
      }

      {user && (user.role === "Finance")
        ? (
          <SidebarGroup>
            <SidebarGroupLabel>Financeiro</SidebarGroupLabel>
            <SidebarMenu>
              <Collapsible>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild tooltip="Solicitações aprovadas">
                    <a href="/request/approved">
                      <LucideCheckCheck />
                      <span className="whitespace-nowrap">Solicitações aprovadas</span>
                      {dataApproved && dataApproved.filter(r => r.approved === 1).length > 0 && (
                        <motion.div
                          className="w-full h-full flex items-center"
                          initial={{ opacity: 0 }}
                          animate={{ opacity: 1 }}
                          transition={{ duration: 0.5 }}
                        >
                          <SidebarMenuBadge>
                            <div className="w-2 h-2 aspect-auto rounded-full bg-success-light" />
                          </SidebarMenuBadge>
                        </motion.div>
                      )}
                    </a>
                  </SidebarMenuButton>
                </SidebarMenuItem>

                <SidebarMenuItem>
                  <SidebarMenuButton asChild tooltip="Solicitações faturadas">
                    <a href="/request/finished">
                      <BanknoteIcon />
                      <span>Solicitações faturadas</span>
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
