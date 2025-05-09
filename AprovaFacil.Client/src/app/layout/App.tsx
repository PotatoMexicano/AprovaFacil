import { ThemeProvider } from '../components/theme-provider';
import { Outlet, useNavigate } from 'react-router-dom';
import { SidebarInset, SidebarProvider, SidebarTrigger } from '../components/ui/sidebar';
import { AppSidebar } from '@/app/components/sidebar/app-sidebar';
import { Separator } from '../components/ui/separator';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbSeparator } from '../components/ui/breadcrumb';
import { BreadcrumbProvider, useBreadcrumb } from '@/app/context/breadcrumb-context';
import React, { useEffect } from 'react';
import { toast, Toaster } from 'sonner';
import { useDispatch } from 'react-redux';
import { authApi, useGetCurrentUserQuery } from '../api/authApiSlice';
import { QueryStatus } from '@reduxjs/toolkit/query';
import { clearUser } from '@/auth/authSlice';
import { Command } from '../components/ui/command';
import { CommandMenu } from '../components/command';

function AppHeader() {
  const { breadcrumbs } = useBreadcrumb();

  const navigate = useNavigate();
  const dispatch = useDispatch();

  const { status, error } = useGetCurrentUserQuery();

  useEffect(() => {
    if (status === QueryStatus.rejected) {
      dispatch(authApi.util.resetApiState());
      dispatch(clearUser());

      toast.error("Sessão Expirada");
      navigate("/login", { replace: true }); // Use navigate instead of window.location.href
    };
  }, [dispatch, error, navigate, status])

  if (status === QueryStatus.pending) {
    return <div></div>
  };

  return (
    <header className="flex h-16 shrink-0 items-center gap-2">
      <div className="flex items-center gap-2 px-4 w-full">
        <SidebarTrigger className="-ml-1" />
        <Separator orientation="vertical" className="mr-2 h-4" />
        <Breadcrumb>
          <BreadcrumbList>
            {breadcrumbs.map((item, index) => (
              <React.Fragment key={index}>
                {index > 0 && <BreadcrumbSeparator />}
                <BreadcrumbItem>
                  <BreadcrumbLink>{String(item)}</BreadcrumbLink>
                </BreadcrumbItem>
              </React.Fragment>
            ))}
          </BreadcrumbList>
        </Breadcrumb>
        {/* <div className='right-0 flex flex-col mt-5 absolute gap-2 px-10 items-center text-muted-foreground opacity-50 select-none'>
          <kbd className="pointer-events-none inline-flex h-5 select-none items-center gap-1 rounded border bg-muted px-1.5 font-mono text-[10px] font-medium text-muted-foreground opacity-100">
            <span className="text-xs">Ctrl + Space</span>
          </kbd>
          <span className="text-xs">Menu de atalhos</span>
        </div> */}
      </div>
    </header>
  );
}

function App() {
  return (
    <div className="flex">
      <Toaster />
      <CommandMenu />
      <ThemeProvider>
        <SidebarProvider className="flex m-auto w-full">
          <BreadcrumbProvider>
            <AppSidebar />
            <SidebarInset>
              <AppHeader />
              <div className="flex flex-1 flex-col gap-4 p-4 pt-0">
                <Outlet />
              </div>
            </SidebarInset>
          </BreadcrumbProvider>
        </SidebarProvider>
      </ThemeProvider>
    </div>
  );
}

export default App;
