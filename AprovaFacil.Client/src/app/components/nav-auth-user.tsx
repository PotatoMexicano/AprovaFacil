"use client"

import {
  ChevronsUpDown,
  Laptop,
  LogOut,
  MoonIcon,
  SunIcon,
} from "lucide-react"

import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@/app/components/ui/avatar"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/app/components/ui/dropdown-menu"
import {
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  useSidebar,
} from "@/app/components/ui/sidebar"
import { UserResponse } from "@/types/auth"
import { authApi, useLogoutMutation } from "../api/authApiSlice"
import { useAppDispatch } from "../store/store"
import { clearUser } from "@/auth/authSlice"
import { useTheme } from "./theme-provider"

interface Props {
  user: UserResponse
}


export function NavAuthUser({ user }: Props) {
  const [funcLogout] = useLogoutMutation();
  const dispatch = useAppDispatch();
  const { isMobile } = useSidebar()
  const { setTheme } = useTheme()

  const logout = async () => {
    dispatch(clearUser());
    dispatch(authApi.util.resetApiState());
    await funcLogout().unwrap();
    window.location.replace('/login')
  };

  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <SidebarMenuButton
              size="lg"
              className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
            >
              <Avatar className="h-8 w-8 rounded-lg">
                <AvatarImage src={user.picture_url} alt={user.full_name} />
                <AvatarFallback className="rounded-lg">{user.full_name}</AvatarFallback>
              </Avatar>
              <div className="grid flex-1 text-left text-sm leading-tight">
                <span className="truncate font-semibold">{user.full_name}</span>
              </div>
              <ChevronsUpDown className="ml-auto size-4" />
            </SidebarMenuButton>
          </DropdownMenuTrigger>
          <DropdownMenuContent
            className="w-[--radix-dropdown-menu-trigger-width] min-w-56 rounded-lg"
            side={isMobile ? "bottom" : "right"}
            align="end"
            sideOffset={4}
          >
            <DropdownMenuLabel className="p-0 font-normal">
              <div className="flex items-center gap-2 px-1 py-1.5 text-left text-sm">
                <Avatar className="h-8 w-8 rounded-lg">
                  <AvatarImage src={user.picture_url} alt={user.full_name} />
                  <AvatarFallback className="rounded-lg">{user.full_name}</AvatarFallback>
                </Avatar>
                <div className="grid flex-1 text-left text-sm leading-tight">
                  <span className="truncate font-semibold">{user.full_name}</span>
                  <span className="truncate font-thin">{user.email}</span>
                </div>
              </div>
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuGroup className="flex flex-row justify-between">
              <DropdownMenuItem className="w-full justify-center" onClick={() => setTheme("light")}><SunIcon /></DropdownMenuItem>
              <DropdownMenuItem className="w-full justify-center" onClick={() => setTheme("dark")}><MoonIcon /></DropdownMenuItem>
              <DropdownMenuItem className="w-full justify-center" onClick={() => setTheme("system")}><Laptop /></DropdownMenuItem>
            </DropdownMenuGroup>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={() => { logout() }}>
              <LogOut />
              Sair
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}
