import React, { createContext, useContext, useState } from "react";

interface BreadcrumbContextType {
  breadcrumbs: string[];
  setBreadcrumbs: React.Dispatch<React.SetStateAction<string[]>>;
}
const BreadcrumbContext = createContext<BreadcrumbContextType | undefined>(undefined);

export const BreadcrumbProvider = ({ children }: { children: React.ReactNode}) => {
  const [breadcrumbs, setBreadcrumbs]= useState<string[]>(["Início"]);

  return (
    <BreadcrumbContext.Provider value={{ breadcrumbs, setBreadcrumbs}}>
      {children}
    </BreadcrumbContext.Provider>
  )
}

export const useBreadcrumb  = () => {
  const context = useContext(BreadcrumbContext);
  if (!context) {
    throw new Error("useBreadcrumb deve ser usado dentro de um BreadcrumbProvider.");
  }
  return context;
};