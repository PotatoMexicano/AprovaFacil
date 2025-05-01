import {
  CommandDialog,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
  CommandShortcut,
} from "@/app/components/ui/command"
import { useIsAdmin, useIsFinance } from "@/lib/utils";
import { BanknoteIcon, Building2Icon, GalleryVerticalEndIcon, InboxIcon, LucideCheckCheck, PackageIcon, PackageOpenIcon, PackagePlusIcon, UserPlus2Icon, Users2Icon, UsersIcon } from "lucide-react"
import { useEffect, useState } from "react"
import { DialogDescription, DialogTitle } from "./ui/dialog";

export function CommandMenu() {
  const [open, setOpen] = useState(false)
  const [search, setSearch] = useState('');

  const isAdmin = useIsAdmin();
  const isFinance = useIsFinance();

  useEffect(() => {
    const down = (e: KeyboardEvent) => {
      if (e.code === "Space" && (e.metaKey || e.ctrlKey)) {
        e.preventDefault()
        setOpen((open) => !open)
      }
    }
    document.addEventListener("keydown", down)
    return () => document.removeEventListener("keydown", down)
  }, [])

  return (
    <CommandDialog open={open} onOpenChange={setOpen}>
      <DialogTitle></DialogTitle>
      <DialogDescription></DialogDescription>
      <CommandInput
        value={search}
        onValueChange={setSearch}
        spellCheck={false}
        autoFocus
        placeholder="Entre com um comando ou pesquisa..."
      />
      <CommandList>

        {!search.startsWith('#') && (
          <CommandEmpty>
            <div className="flex flex-col items-center text-muted-foreground text-lg gap-2">
              <InboxIcon size={30} />Nenhum resultado.
            </div>
          </CommandEmpty>
        )}

        {search.startsWith('#') && (
          <CommandGroup heading="Solicitações">
            <CommandItem asChild>
              <a href={`/request/${search.replace('#', '')}`}>
                Solicitação {search}
              </a>
            </CommandItem>
          </CommandGroup>
        )}

        <CommandGroup heading="Solicitação">
          <CommandItem keywords={['nova', 'adicionar', 'registrar']} asChild>
            <a href="/request/register">
              <PackagePlusIcon strokeWidth={1.5} absoluteStrokeWidth />
              <span>Registrar solicitação</span>
              <CommandShortcut>⌘1</CommandShortcut>
            </a>
          </CommandItem>

          <CommandItem asChild>
            <a href="/request/">
              <PackageOpenIcon strokeWidth={1.5} />
              <span>Minhas solicitações</span>
              <CommandShortcut>⌘2</CommandShortcut>
            </a>
          </CommandItem>

          {isAdmin && (
            <>
              <CommandItem asChild>
                <a href="/request/pending">
                  <PackageIcon strokeWidth={1.75} absoluteStrokeWidth />
                  <span>Solicitações pendentes</span>
                </a>
              </CommandItem>

              <CommandItem keywords={['todas', 'tudo', 'geral', 'completo']} asChild>
                <a href="/request/all">
                  <GalleryVerticalEndIcon strokeWidth={1.5} absoluteStrokeWidth />
                  <span>Histórico de solicitações</span>
                </a>
              </CommandItem>
            </>
          )}

        </CommandGroup>

        <CommandSeparator />
        {isFinance && (
          <>
            <CommandGroup heading="Financeiro">
              <CommandItem asChild>
                <a href="/request/approved">
                  <LucideCheckCheck strokeWidth={1.75} absoluteStrokeWidth />
                  <span>Solicitações aprovadas</span>
                  <CommandShortcut>⌘3</CommandShortcut>
                </a>
              </CommandItem>
              <CommandItem keywords={['financeiro', 'encerradas', 'concluidas']} asChild>
                <a href="/request/finished">
                  <BanknoteIcon strokeWidth={1.75} absoluteStrokeWidth />
                  <span>Solicitações faturadas</span>
                </a>
              </CommandItem>
            </CommandGroup>
            <CommandSeparator />
          </>
        )}

        <CommandGroup heading="Empresas">
          <CommandItem asChild>
            <a href="/company/register">
              <Building2Icon strokeWidth={1.75} absoluteStrokeWidth />
              <span>Registrar empresa</span>
              <CommandShortcut>⌘4</CommandShortcut>
            </a>
          </CommandItem>
          <CommandItem asChild>
            <a href="/company">
              <Building2Icon strokeWidth={1.75} absoluteStrokeWidth />
              <span>Visualizar empresas</span>
            </a>
          </CommandItem>
        </CommandGroup>

        <CommandSeparator />

        <CommandGroup heading="Usuários">
          <CommandItem asChild>
            <a href="/users/register">
              <UserPlus2Icon strokeWidth={1.75} absoluteStrokeWidth />
              <span>Novo usuário</span>
            </a>
          </CommandItem>
          <CommandItem asChild>
            <a href="/users">
              <UsersIcon strokeWidth={1.75} absoluteStrokeWidth />
              <span>Visualizar usuários</span>
            </a>
          </CommandItem>
        </CommandGroup>

      </CommandList>
    </CommandDialog >
  )
}