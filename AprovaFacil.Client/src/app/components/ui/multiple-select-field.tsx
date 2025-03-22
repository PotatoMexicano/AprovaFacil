"use client"

import { useState, useEffect } from "react"
import { Check, User } from "lucide-react"
import { Button } from "@/app/components/ui/button"
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/app/components/ui/command"
import { FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/app/components/ui/form"
import { Popover, PopoverContent, PopoverTrigger } from "@/app/components/ui/popover"
import { Avatar, AvatarFallback, AvatarImage } from "@/app/components/ui/avatar"
import { Badge } from "@/app/components/ui/badge"
import { cn } from "@/lib/utils"
import { UserResponse } from "@/types/auth"

// Função auxiliar para obter iniciais do nome
const getInitials = (name: string) => {
  return name
    .split(" ")
    .map((part) => part[0])
    .join("")
    .substring(0, 2)
    .toUpperCase()
}

export function MultiSelectUserField({
  form,
  users = [],
  isUsersFetching = false,
}: {
  form: any
  users?: UserResponse[]
  isUsersFetching?: boolean
}) {
  const [openPopoverUser, setOpenPopoverUser] = useState(false)
  const [selectedIds, setSelectedIds] = useState<number[]>([])

  const {formState: {isSubmitSuccessful}} = form;

  useEffect(() => {
    if (isSubmitSuccessful) {
      // Após o sucesso e o reset, podemos zerar estados adicionais aqui
      setOpenPopoverUser(false);
      setSelectedIds([]);
      // Opcional: Redefinir outros estados fora do formulário, se existirem
      // Exemplo: setSomeExternalState(initialValue);
    }
  }, [isSubmitSuccessful]);

  // Sincroniza o estado local com o valor do formulário
  useEffect(() => {
    const currentValue = form.getValues("directorsIds") || []
    setSelectedIds(Array.isArray(currentValue) ? currentValue : [])
  }, [form])

  // Função para alternar a seleção de um usuário
  const toggleUserSelection = (userId: number) => {
    const newSelectedIds = selectedIds.includes(userId)
      ? selectedIds.filter((id) => id !== userId)
      : [...selectedIds, userId]

    setSelectedIds(newSelectedIds)
    form.setValue("directorsIds", newSelectedIds)
    form.trigger("directorsIds")
  }

  // Filtra apenas diretores
  const diretores = users?.filter((x) => x.role === "Diretor") || []

  // Obtém os nomes dos diretores selecionados
  const selectedDiretoresNames = diretores.filter((user) => selectedIds.includes(user.id)).map((user) => user.full_name)

  // Texto a ser exibido no botão
  const buttonText =
    selectedIds.length > 0
      ? selectedIds.length === 1
        ? selectedDiretoresNames[0]
        : `${selectedIds.length} diretores selecionados`
      : "Selecione diretores"

  return (
    <FormField
      control={form.control}
      name="directorsIds"
      render={({ field }) => (
        <FormItem className="flex flex-col">
          <FormLabel>Diretores</FormLabel>
          <Popover open={openPopoverUser} onOpenChange={setOpenPopoverUser}>
            <PopoverTrigger asChild>
              <FormControl>
                <Button
                  variant="outline"
                  role="combobox"
                  className={cn(
                    "justify-between font-normal min-h-[18px] ",
                    !field.value?.length && "text-muted-foreground",
                  )}
                >
                  <div className="flex flex-wrap gap-1 items-center max-w-[90%] overflow-hidden">
                    {selectedIds.length > 0 ? (
                      selectedIds.length === 1 ? (
                        <span className="truncate">{buttonText}</span>
                      ) : (
                        <>
                          {selectedDiretoresNames.slice(0, 2).map((name) => (
                            <Badge key={name} variant="secondary" className="truncate max-w-[150px]">
                              {name}
                            </Badge>
                          ))}
                          {selectedIds.length > 2 && <Badge variant="secondary">+{selectedIds.length - 2}</Badge>}
                        </>
                      )
                    ) : (
                      <span>{buttonText}</span>
                    )}
                  </div>
                  <User className="text-foreground opacity-50 ml-auto flex-shrink-0" />
                </Button>
              </FormControl>
            </PopoverTrigger>
            <PopoverContent className="p-0 w-96" align="center">
              <Command>
                <CommandInput placeholder="Buscar diretores..." className="h-9" />
                <CommandList>
                  {isUsersFetching && <CommandEmpty>Carregando pessoas...</CommandEmpty>}
                  {!isUsersFetching && diretores.length === 0 && <CommandEmpty>Nenhum diretor encontrado.</CommandEmpty>}
                  <CommandGroup>
                    {diretores.map((user) => (
                      <CommandItem
                        tabIndex={3}
                        key={user.id}
                        value={user.full_name}
                        onSelect={() => {
                          toggleUserSelection(user.id)
                          // Não fechamos o popover para permitir múltiplas seleções
                        }}
                      >
                        <Avatar>
                          <AvatarImage src={user.picture_url} />
                          <AvatarFallback>{getInitials(user.full_name)}</AvatarFallback>
                        </Avatar>
                        <div
                          className={cn(
                            "flex flex-col text-[18px] p-2",
                            selectedIds.includes(user.id) ? "font-semibold" : "font-normal",
                          )}
                        >
                          <p>{user.full_name}</p>
                          <small className="text-[14px] font-normal text-foreground/80">
                            {user.role} | {user.department}
                          </small>
                        </div>
                        <Check className={cn("ml-auto", selectedIds.includes(user.id) ? "opacity-100" : "opacity-0")} />
                      </CommandItem>
                    ))}
                  </CommandGroup>
                </CommandList>
              </Command>
            </PopoverContent>
          </Popover>
          <FormMessage>{form.formState.errors.diretores?.message}</FormMessage>
        </FormItem>
      )}
    />
  )
}

