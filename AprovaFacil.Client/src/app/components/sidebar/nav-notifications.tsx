import * as React from "react"

import {
  SidebarGroup,
  SidebarGroupContent,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/app/components/ui/sidebar"
import { WorkflowModal } from "./helper-modal"
import { Bell } from "lucide-react"
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle, SheetTrigger } from "../ui/sheet"
import { NotificationsList } from "../notifications/notification-list"
import { useState } from "react"
import { useGetNotificationsQuery } from "@/app/api/notificationApiSlice"
import { motion } from "framer-motion"

export function NavNotifications({
  ...props
}: {} & React.ComponentPropsWithoutRef<typeof SidebarGroup>) {

  const { data } = useGetNotificationsQuery();
  const [open, setOpen] = useState(false);

  return (
    <SidebarGroup {...props}>
      <SidebarGroupContent>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton asChild size="sm">
              <WorkflowModal />
            </SidebarMenuButton>
          </SidebarMenuItem>

          <SidebarMenuItem>
            <Sheet open={open} onOpenChange={setOpen}>
              <SheetTrigger asChild>
                <SidebarMenuButton asChild tooltip="Notificações">
                  <div className="cursor-pointer">
                    <Bell />
                    <span>Notificações</span>
                    {data && data.length > 0 && (
                      <motion.div
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{ duration: 0.5 }}
                      >
                        <div className="absolute w-2 h-2 aspect-auto rounded-full bg-orange-500 right-2"></div>
                      </motion.div>
                    )}
                  </div>
                </SidebarMenuButton>
              </SheetTrigger>
              <SheetContent>
                <SheetHeader>
                  <SheetTitle>Últimas notificações</SheetTitle>
                  <SheetDescription>
                    <NotificationsList />
                  </SheetDescription>
                </SheetHeader>
              </SheetContent>
            </Sheet>
          </SidebarMenuItem>

        </SidebarMenu>
      </SidebarGroupContent>
    </SidebarGroup>
  )
}
