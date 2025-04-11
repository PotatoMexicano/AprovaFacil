import { Button } from "@/app/components/ui/button";
import { Calendar } from "@/app/components/ui/calendar";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/app/components/ui/form";
import { Popover, PopoverContent, PopoverTrigger } from "@/app/components/ui/popover";
import { Textarea } from "@/app/components/ui/textarea";
import { zodResolver } from "@hookform/resolvers/zod";
import { format } from "date-fns";
import { BuildingIcon, CalendarIcon, Check, FileText, UploadCloud, User } from "lucide-react";
import { useForm } from "react-hook-form";
import { ptBR } from "date-fns/locale"
import { z } from "zod";
import { useGetCompaniesQuery } from "@/app/api/companyApiSlice";
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/app/components/ui/command";
import { useEffect, useState } from "react";
import { cn } from "@/lib/utils";
import { Separator } from "@/app/components/ui/separator";
import CurrencyFormField from "@/app/components/ui/currency-form-field";
import { Avatar, AvatarFallback, AvatarImage } from "@/app/components/ui/avatar";
import FileUpload from "@/app/components/ui/file-upload";
import { useIsMobile } from "@/hooks/use-mobile";
import { MultiSelectUserField } from "@/app/components/ui/multiple-select-field";
import { useRegisterRequestMutation } from "@/app/api/requestApiSlice";
import formSchema from "@/app/schemas/requestSchema";
import { useGetEnabledUsersQuery } from "@/app/api/userApiSlice";
import { toast } from "sonner";
import ButtonSuccess from "@/app/components/ui/button-success";
import { useBreadcrumb } from "@/app/context/breadcrumb-context";

const getInitials = (fullName: string) => {
  if (!fullName) return "";

  // Divide o nome em palavras
  const parts = fullName.trim().split(" ");

  // Verifica se há pelo menos duas partes (nome e sobrenome)
  if (parts.length < 2) return parts[0]?.charAt(0).toUpperCase() || "";

  // Pega a primeira letra do primeiro nome e do primeiro sobrenome
  const firstNameInitial = parts[0].charAt(0).toUpperCase();
  const lastNameInitial = parts[1].charAt(0).toUpperCase();

  // Combina as iniciais
  return `${firstNameInitial}${lastNameInitial}`;
};

export default function NewRequestPage() {

  const { setBreadcrumbs } = useBreadcrumb()

  const { data: companies, isFetching: isCompaniesFetching } = useGetCompaniesQuery();
  const { data: users, isFetching: isUsersFetching } = useGetEnabledUsersQuery();
  const [registerRequest, { isSuccess, isError, isLoading, error }] = useRegisterRequestMutation();
  const [registerSuccess, setRegisterSuccess] = useState<boolean | undefined>(undefined)

  useEffect(() => {
      setRegisterSuccess(isSuccess)
  
      const timer = setTimeout(() => {
        setRegisterSuccess(undefined)
      }, 3500)
  
      return () => clearTimeout(timer)
    }, [isSuccess, isError]);

  useEffect(() => {
    setBreadcrumbs(["Início", "Solicitação", "Adicionar"])
  }, [setBreadcrumbs])

  const form = useForm({
    resolver: zodResolver(formSchema),
    defaultValues: {
      companyId: 0,
      paymentDate: undefined,
      amount: 0,
      note: "",
      invoice: undefined,
      budget: undefined,
      managerId: 0,
      directorsIds: [],
    }
  });

  const { reset } = form;
  const [openPopoverUser, setOpenPopoverUser] = useState(false);
  const [idPopoverUser, setIdPopoverUser] = useState<number | null>(null);
  const [openPopoverCompany, setOpenPopoverCompany] = useState(false);
  const [idPopoverCompany, setIdPopoverCompany] = useState<number | null>(null);

  const isMobile = useIsMobile();

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    try {
      await registerRequest(values).unwrap();

      setRegisterSuccess(isSuccess);

      toast.success('Requisição cadastrada !');

      reset({
        companyId: undefined,
        paymentDate: undefined,
        amount: 0,
        note: "",
        invoice: undefined,
        budget: undefined,
        managerId: 0,
        directorsIds: [],
      });
      setIdPopoverCompany(null);
      setIdPopoverUser(null);

    } catch (err) {
      console.error(`Erro ao registrar solicitação:`, error);
      if (error) {
        toast.error(`Falha ao registrar solicitação`)
      }
    }
  }

  return (
    <>
      <Card className="col-span-12 flex flex-col shadow-none border-0">
        <CardHeader>
          <CardTitle>Nova requisição para aprovação.</CardTitle>
          <CardDescription>Dados da nota fiscal para envio.</CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)}>

              <div className="grid grid-cols-12 gap-5">

                <div className="col-span-12 md:col-span-8 flex flex-col gap-10">
                  <div className="grid grid-cols-1 space-y-2">
                    <FormField
                      control={form.control}
                      name="invoice"
                      render={({ field: { onChange, value, ...field } }) => (
                        <FormItem>
                          <FormLabel>Nota Fiscal</FormLabel>
                          <FormControl>
                            <FileUpload {...field} value={value} onChange={onChange} label="Nota Fiscal" icon={UploadCloud} tabIndex={1} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>

                  <div className="grid grid-cols-1 space-y-2">
                    <FormField
                      control={form.control}
                      name="budget"
                      render={({ field: { onChange, value, ...field } }) => (
                        <FormItem>
                          <FormLabel>Orçamento</FormLabel>
                          <FormControl>
                            <FileUpload {...field} value={value} onChange={onChange} label="Orçamento" icon={FileText} tabIndex={2} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>

                  <div>
                    {!isMobile && (
                      <div className="flex w-full justify-start">
                        <ButtonSuccess
                          isLoading={isLoading}
                          isSuccess={registerSuccess}
                          defaultText="Cadastrar Requisição"
                          loadingText="Cadastrando..."
                          successText="Requisição Cadastrada!"
                        />
                      </div>
                    )}
                  </div>

                </div>

                <div className="p-0 md:px-10 justify-self-end flex flex-col col-span-12 md:col-span-4 gap-5 w-full">

                  <FormField
                    control={form.control}
                    name="managerId"
                    render={({ field }) => (
                      <FormItem className="flex flex-col">
                        <FormLabel>Gerente</FormLabel>
                        <Popover open={openPopoverUser} onOpenChange={setOpenPopoverUser}>
                          <PopoverTrigger asChild>
                            <FormControl>
                              <Button
                                variant={"outline"}
                                role="combobox"
                                className={cn(
                                  "justify-between font-normal",
                                  !field.value && "text-muted-foreground"
                                )}>
                                {field.value
                                  ? users?.find(
                                    (user) => user.id === Number(field.value))?.full_name
                                  : "Selecione uma pessoa"}
                                <User className="text-foreground opacity-50" />
                              </Button>
                            </FormControl>
                          </PopoverTrigger>
                          <PopoverContent className="p-0 w-96" align="center">
                            <Command>
                              <CommandInput placeholder="Buscar pessoas..." className="h-9" />
                              <CommandList>
                                {isUsersFetching && (<CommandEmpty>Carregando pessoas...</CommandEmpty>)}
                                {!isUsersFetching && users && (<CommandEmpty>Nenhuma pessoa encontrada.</CommandEmpty>)}
                                <CommandGroup>
                                  {users?.filter(x => x.role === "Manager")?.map((user) => (
                                    <CommandItem
                                      tabIndex={3}
                                      key={user.id}
                                      value={user.full_name}
                                      onSelect={() => {
                                        form.setValue("managerId", user.id)
                                        form.trigger("managerId")
                                        setOpenPopoverUser(false)
                                        setIdPopoverUser(user.id)
                                      }}>
                                      <Avatar>
                                        <AvatarImage src={user.picture_url} />
                                        <AvatarFallback>{getInitials(user.full_name)}</AvatarFallback>
                                      </Avatar>
                                      <div className={cn("flex flex-col text-[18px] p-2",
                                        idPopoverUser === user.id
                                          ? "font-semibold"
                                          : "font-normal"
                                      )}>
                                        <p>{user.full_name}</p>
                                        <small className="text-[14px] font-normal text-foreground/80">{user.role_label} | {user.department_label}</small>
                                      </div>
                                      <Check
                                        className={cn(
                                          "ml-auto",
                                          idPopoverUser === user.id ? "opacity-100" : "opacity-0"
                                        )} />
                                    </CommandItem>
                                  ))}
                                </CommandGroup>
                              </CommandList>
                            </Command>
                          </PopoverContent>
                        </Popover>
                        <FormMessage>{form.formState.errors.managerId?.message}</FormMessage>
                      </FormItem>
                    )}
                  />
                  <Separator orientation="horizontal" className="w-full" />

                  <MultiSelectUserField form={form} isUsersFetching={isUsersFetching} users={users} />

                  <Separator orientation="horizontal" className="w-full" />

                  <FormField
                    control={form.control}
                    name="companyId"
                    render={({ field }) => (
                      <FormItem className="flex flex-col">
                        <FormLabel>Empresa</FormLabel>
                        <Popover open={openPopoverCompany} onOpenChange={setOpenPopoverCompany}>
                          <PopoverTrigger asChild>
                            <FormControl>
                              <Button
                                variant={"outline"}
                                role="combobox"
                                className={cn(
                                  "justify-between font-normal",
                                  !field.value && "text-muted-foreground"
                                )}>
                                {field.value
                                  ? companies?.find(
                                    (company) => company.id === Number(field.value))?.trade_name
                                  : "Selecione uma empresa"}
                                <BuildingIcon className="text-foreground opacity-50" />
                              </Button>
                            </FormControl>
                          </PopoverTrigger>
                          <PopoverContent className="p-0 w-96" align="center">
                            <Command>
                              <CommandInput placeholder="Buscar empresas..." className="h-9" />
                              <CommandList>
                                {isCompaniesFetching && (<CommandEmpty>Carregando empresas...</CommandEmpty>)}
                                {!isCompaniesFetching && companies && (<CommandEmpty>Nenhuma empresa encontrada.</CommandEmpty>)}
                                <CommandGroup>
                                  {companies?.map((company) => (
                                    <CommandItem
                                      key={company.id}
                                      value={company.trade_name}
                                      onSelect={() => {
                                        form.setValue("companyId", company.id)
                                        setOpenPopoverCompany(false)
                                        setIdPopoverCompany(company.id)
                                        form.trigger("companyId")
                                      }}>
                                      <div className={cn("p-2 w-full",
                                        idPopoverCompany === company.id ? "font-semibold" : ""
                                      )}>
                                        {company.trade_name}
                                      </div>
                                      <Check
                                        className={cn(
                                          "ml-auto",
                                          idPopoverCompany === company.id ? "opacity-100" : "opacity-0"
                                        )} />
                                    </CommandItem>
                                  ))}
                                </CommandGroup>
                              </CommandList>
                            </Command>
                          </PopoverContent>
                        </Popover>
                        <FormMessage>{form.formState.errors.companyId?.message}</FormMessage>
                      </FormItem>
                    )}
                  />

                  <Separator orientation="horizontal" className="w-full" />

                  <FormField
                    control={form.control}
                    name="paymentDate"
                    render={({ field }) => (
                      <FormItem className="flex flex-col">
                        <FormLabel>Data pagamento nota fiscal</FormLabel>
                        <FormControl>
                          <Popover>
                            <PopoverTrigger asChild>
                              <FormControl>
                                <Button
                                  variant="outline"
                                  className="w-full justify-start text-left font-normal"
                                >
                                  {field.value ? format(field.value, "dd 'de' MMMM 'de' yyyy", { locale: ptBR }) : "Selecione uma data"}
                                  <CalendarIcon className="ml-auto h-4 w-4 text-foreground opacity-50" />
                                </Button>
                              </FormControl>
                            </PopoverTrigger>
                            <PopoverContent className="w-auto p-0" align="center">
                              <Calendar
                                required
                                disabled={{ before: new Date() }}
                                mode="single"
                                selected={field.value}
                                onSelect={field.onChange}
                                initialFocus
                                locale={ptBR}
                              />
                            </PopoverContent>
                          </Popover>
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <Separator orientation="horizontal" className="w-full" />

                  <CurrencyFormField form={form} />

                  <Separator orientation="horizontal" className="w-full" />

                  <FormField
                    control={form.control}
                    name="note"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Observação</FormLabel>
                        <FormControl>
                          <Textarea placeholder="Ex: Nota deve ser aprovada antes de..." rows={2} {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  {isMobile && (
                    <div className="flex w-full justify-start">
                      <ButtonSuccess
                        isLoading={isLoading}
                        isSuccess={registerSuccess}
                        defaultText="Cadastrar Requisição"
                        loadingText="Cadastrando..."
                        successText="Requisição Cadastrada!"
                      />
                    </div>
                  )}
                </div>


              </div>

            </form>
          </Form>
        </CardContent>
      </Card>
    </>
  )
}