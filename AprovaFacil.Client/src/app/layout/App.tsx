import { ThemeProvider } from '../components/theme-provider'
import { Outlet } from 'react-router-dom'
import { SidebarInset, SidebarProvider, SidebarTrigger } from '../components/ui/sidebar'
import { AppSidebar } from '../components/app-sidebar'
import { Separator } from '../components/ui/separator'
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbSeparator } from '../components/ui/breadcrumb'
import { BreadcrumbProvider, useBreadcrumb } from '../context/breadkcrumb-context'
import React from 'react'
import { Toaster } from 'sonner'

function AppHeader() {

  const { breadcrumbs } = useBreadcrumb();

  return (
    <header className="flex h-16 shrink-0 items-center gap-2">
      <div className="flex items-center gap-2 px-4">
        <SidebarTrigger className="-ml-1" />
        <Separator orientation="vertical" className="mr-2 h-4" />
        <Breadcrumb>
          <BreadcrumbList>
            {breadcrumbs.map((item, index) => (
              <React.Fragment key={index}>
                {index > 0 ? (<BreadcrumbSeparator />) : null}
                <BreadcrumbItem key={index}>
                  <BreadcrumbLink>{item}</BreadcrumbLink>
                </BreadcrumbItem>
              </React.Fragment>
            ))}
          </BreadcrumbList>
        </Breadcrumb>
      </div>
    </header>
  );
}

function App() {
  return (
    <div className="flex w-screen">
      <ThemeProvider defaultTheme="system" storageKey="vite-ui-theme">
        <Toaster />
        <BreadcrumbProvider>
          <SidebarProvider className="flex m-auto w-full">
            <AppSidebar />
            <SidebarInset>
              <AppHeader />
              <div className="flex flex-1 flex-col gap-4 p-4 pt-0">
                <Outlet />
              </div>
            </SidebarInset>
          </SidebarProvider>
        </BreadcrumbProvider>
      </ThemeProvider>
    </div>
  );
}

export default App
