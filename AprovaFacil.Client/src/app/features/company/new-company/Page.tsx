import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Input } from "@/app/components/ui/input";
import { useBreadcrumb } from "@/app/context/breadkcrumb-context"
import { useEffect, useState } from "react";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/app/components/ui/form";
import { useGetCepQuery } from "../../../api/cepApiSlice";
import { useMaskito } from "@maskito/react"
import { Skeleton } from "@/app/components/ui/skeleton";
import { Button } from "@/app/components/ui/button";
import type { MaskitoOptions } from '@maskito/core';
import { Separator } from "@/app/components/ui/separator";
import { useIsMobile } from "@/hooks/use-mobile";
import formSchema from "@/app/schemas/companySchema";
import { useRegisterCompanyMutation } from "@/app/api/companyApiSlice";
import ButtonSuccess from "@/app/components/ui/button-success";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";

export default function NewCompanyPage() {

  const isMobile = useIsMobile();
  const navigate = useNavigate()

  const [registerCompany, { isLoading, isSuccess, isError, error }] = useRegisterCompanyMutation();
  const [registerSuccess, setRegisterSuccess] = useState<boolean | undefined>(undefined);

  const maskedCnpjRef = useMaskito({
    options: {
      mask: [
        /\d/, /\d/, ".",
        /\d/, /\d/, /\d/, ".",
        /\d/, /\d/, /\d/, "/",
        /\d/, /\d/, /\d/, /\d/, "-",
        /\d/, /\d/
      ],
    } satisfies MaskitoOptions
  });

  const maskedPhoneRef = useMaskito({
    options: {
      mask: [
        "(",
        /\d/, /\d/, ")", " ",
        /\d/, /\d/, /\d/, /\d/, /\d/,
        "-",
        /\d/, /\d/, /\d/, /\d/
      ]
    } satisfies MaskitoOptions
  });

  const maskedPostalCodeRef = useMaskito({
    options: {
      mask: [
        /\d/, /\d/, /\d/, /\d/, /\d/,
        "-",
        /\d/, /\d/, /\d/
      ]
    } satisfies MaskitoOptions
  })

  const { setBreadcrumbs } = useBreadcrumb();

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      id: 0,
      postal_code: "",
      state: "",
      city: "",
      street: "",
      neighborhood: "",
      number: "",
      complement: "",
      trade_name: "",
      legal_name: "",
      cnpj: "",
      phone: "",
      email: ""
    },
  });

  const [postalCodeSearch, setPostalCodeSearch] = useState(form.getValues("postal_code"));

  const { data: address, isError: isPostalCodeError, isLoading: isPostalCodeLoading, isFetching: isPostalCodeFetching } = useGetCepQuery(postalCodeSearch, {
    skip: !postalCodeSearch,
  });

  const buscarCEP = () => {
    const postalCode = form.getValues("postal_code").replace(/\D/g, "") || "";
    if (postalCode.length === 8) {
      setPostalCodeSearch(postalCode)
    }
  }

  useEffect(() => {
    setRegisterSuccess(isSuccess)

    const timer = setTimeout(() => {
      setRegisterSuccess(undefined)
    }, 3500)

    return () => clearTimeout(timer)
  }, [isSuccess, isError, navigate]);

  useEffect(() => {
    if (isPostalCodeError) {
      toast({
        title: "Falha ao encontrar CEP",
        description: `Não foi possível encontrar dados do CEP - ${form.getValues("postal_code")}`
      });

      form.setValue('state', ``);
      form.setValue('city', ``);
      form.setValue('neighborhood', ``);
      form.setValue('street', ``);

      form.trigger(["state", "city", "neighborhood", "street"])
    }
  }, [isPostalCodeError]);

  useEffect(() => {
    if (address) {
      form.setValue('state', address.estado);
      form.setValue('city', address.localidade);
      form.setValue('neighborhood', address.bairro);
      form.setValue('street', address.logradouro);

      form.trigger(["state", "city", "neighborhood", "street"])
    }
  }, [address, postalCodeSearch]);

  useEffect(() => {
    setBreadcrumbs(["Início", "Empresas", "Adicionar"]); // Define os breadcrumbs da página atual
  }, [setBreadcrumbs]);

  async function onSubmit(values: z.infer<typeof formSchema>) {
    try {
      await registerCompany(values).unwrap();

      setRegisterSuccess(isSuccess);

      toast.success("Empresa registrada com sucesso.");

      setTimeout(() => {
        navigate("/company")
      }, 2000);

    } catch (err) {
      console.error(`Erro ao registrar empresa:`, error);
      if (error) {
        toast.error(`Falha ao registrar empresa`)
      }
    }
  }

  return (
    <>
      <div >
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="grid grid-cols-12 gap-5">

            <Card className="col-span-12 flex flex-col shadow-none border-0">
              <CardHeader>
                <CardTitle>Empresa para emissão de notas.</CardTitle>
                <CardDescription>Dados da empresa emissora.</CardDescription>
              </CardHeader>
              <CardContent>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="flex flex-col space-y-2">
                    <div>
                      <FormField
                        control={form.control}
                        name="cnpj"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>CNPJ</FormLabel>
                            <FormControl>
                              <Input
                                {...field}
                                ref={maskedCnpjRef}
                                placeholder="12.345.678/0001-95"
                                onInput={(e) => form.setValue("cnpj", e.currentTarget.value)}
                                onBlur={() => form.trigger("cnpj")}
                              />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                    </div>

                    <div>
                      <FormField
                        control={form.control}
                        name="trade_name"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Nome Fantasia</FormLabel>
                            <FormControl>
                              <Input placeholder="Limões e Laranjas" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                    </div>

                    <div>
                      <FormField
                        control={form.control}
                        name="legal_name"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Razão Social</FormLabel>
                            <FormControl>
                              <Input placeholder="Limões Varejo LTDA" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-2">
                      <FormField
                        control={form.control}
                        name="phone"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Telefone</FormLabel>
                            <FormControl>
                              <Input
                                {...field}
                                ref={maskedPhoneRef}
                                placeholder="(19) 99897-0630"
                                onInput={(e) => form.setValue("phone", e.currentTarget.value)}
                                onBlur={() => form.trigger("phone")}
                              />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />

                      <FormField
                        control={form.control}
                        name="email"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Email</FormLabel>
                            <FormControl>
                              <Input placeholder="Ex: limões.varejo@contato.com" {...field} />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />

                    </div>

                  </div>

                  {isMobile && (<Separator className="mt-4" />)}

                  <div className="flex flex-col space-y-2">
                    <div>
                      <FormField
                        control={form.control}
                        name="postal_code"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>CEP</FormLabel>
                            <FormControl>
                              <div className="flex w-full items-center space-x-2">
                                <Input
                                  {...field}
                                  ref={maskedPostalCodeRef}
                                  placeholder="13417-570"
                                  onInput={(e) => form.setValue("postal_code", e.currentTarget.value)}
                                  onBlur={() => form.trigger("postal_code")}
                                />
                                <Button type="button" variant={"outline"} onClick={() => buscarCEP()}>{isPostalCodeFetching ? "Buscando" : "Buscar"}</Button>
                              </div>
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                    </div>

                    <div className=" grid grid-cols-1 md:grid-cols-2 gap-2">
                      <div>
                        <FormField
                          control={form.control}
                          name="state"
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>Estado</FormLabel>
                              <FormControl>
                                {isPostalCodeLoading || isPostalCodeFetching
                                  ? (<Skeleton className="py-1 px-3 h-9 w-full rounded-md" />)
                                  : (<Input placeholder="Ex: São Paulo" {...field} />)}
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                      </div>

                      <div>
                        <FormField
                          control={form.control}
                          name="city"
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>Cidade</FormLabel>
                              <FormControl>
                                {isPostalCodeLoading || isPostalCodeFetching
                                  ? (<Skeleton className="py-1 px-3 h-9 w-full rounded-md" />)
                                  : (<Input placeholder="Ex: Piracicaba" {...field} />)}
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                      </div>
                    </div>

                    <div className=" grid grid-cols-1 md:grid-cols-2 gap-2">

                      <div>
                        <FormField
                          control={form.control}
                          name="street"
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>Logradouro</FormLabel>
                              <FormControl>
                                {isPostalCodeLoading || isPostalCodeFetching
                                  ? (<Skeleton className="py-1 px-3 h-9 w-full rounded-md" />)
                                  : (<Input placeholder="Ex: Rua Brasilia" {...field} />)}
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                      </div>

                      <div>
                        <FormField
                          control={form.control}
                          name="number"
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>Número</FormLabel>
                              <FormControl>
                                <Input placeholder="Ex: 123" {...field} />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                      </div>

                    </div>

                    <div className=" grid grid-cols-1 md:grid-cols-2 gap-2">
                      <div>
                        <FormField
                          control={form.control}
                          name="neighborhood"
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>Bairro</FormLabel>
                              <FormControl>
                                {isPostalCodeLoading || isPostalCodeFetching
                                  ? (<Skeleton className="py-1 px-3 h-9 w-full rounded-md" />)
                                  : (<Input placeholder="Ex: Nova América" {...field} />)}
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                      </div>

                      <div>
                        <FormField
                          control={form.control}
                          name="complement"
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>Complemento</FormLabel>
                              <FormControl>
                                <Input placeholder="Ex: Galpão 13A" {...field} />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                      </div>
                    </div>

                  </div>
                </div>

                <div className="pt-5">
                  <ButtonSuccess
                    isLoading={isLoading}
                    isSuccess={registerSuccess}
                    defaultText="Cadastrar Empresa"
                    loadingText="Cadastrando..."
                    successText="Sucesso !" />
                </div>
              </CardContent>
            </Card>
          </form>
        </Form>

      </div>
    </>
  )
}