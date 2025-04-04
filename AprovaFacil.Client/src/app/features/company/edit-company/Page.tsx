import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Input } from "@/app/components/ui/input";
import { useBreadcrumb } from "@/app/context/breadkcrumb-context"
import { useEffect, useState } from "react";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/app/components/ui/form";
import { useGetCepQuery } from "../../../api/cepApiSlice";
import { toast } from "@/hooks/use-toast";
import { useMaskito } from "@maskito/react"
import { Skeleton } from "@/app/components/ui/skeleton";
import { Button } from "@/app/components/ui/button";
import type { MaskitoOptions } from '@maskito/core';
import { useParams } from "react-router-dom";
import { useGetCompanyQuery, useUpdateCompanyMutation } from "@/app/api/companyApiSlice";
import { Badge } from "@/app/components/ui/badge";
import { useIsMobile } from "@/hooks/use-mobile";
import { Separator } from "@/app/components/ui/separator";
import ButtonSuccess from "@/app/components/ui/button-success";
import formSchema from "@/app/schemas/companySchema";

export default function EditCompanyPage() {

  const { id } = useParams<{ id: string }>();
  const isMobile = useIsMobile();

  const { data: company, isSuccess: isCompanySuccess } = useGetCompanyQuery(id || "");
  const [updateCompany, { isLoading: isLoadingUpdateCompany, isSuccess: isSuccessUpdateCompany, isError: isErrorUpdateCompany }] = useUpdateCompanyMutation();
  const [updateSuccess, setUpdateSuccess] = useState<boolean | undefined>(undefined);

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

  const maskedCepRef = useMaskito({
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
      postal_code: "",
      state: "",
      city: "",
      street: "",
      neighborhood: "",
      number: "0",
      complement: "",
      trade_name: "",
      legal_name: "",
      cnpj: "",
      phone: "",
      email: ""
    },
  });

  const { reset } = form;

  useEffect(() => {
    if (isCompanySuccess && company) {
      reset(company);
    }
  }, [company, isCompanySuccess, reset, form])

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
    setUpdateSuccess(isSuccessUpdateCompany);
    if (isSuccessUpdateCompany) {
      toast({
        title: "Empresa atualizada",
        description: "Os dados da empresa foram atualizados com sucesso."
      });
    }
    if (isErrorUpdateCompany) {
      toast({
        title: "Falha ao atualizar empresa",
        description: "Não foi possível atualizar os dados da empresa."
      })
    }
    
   const timer = setTimeout(() => {
    setUpdateSuccess(undefined);
   }, 3500);

   return () => clearTimeout(timer);
  }, [isSuccessUpdateCompany, isErrorUpdateCompany]);

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
    setBreadcrumbs(["Início", "Empresa", "Editar"]); // Define os breadcrumbs da página atual
  }, [setBreadcrumbs]);

  async function onSubmit(values: z.infer<typeof formSchema>) {
    try {
      await updateCompany(values).unwrap();
    } catch (error) {

      console.error(`Erro ao atualizar empresa:`, error);

      if (error instanceof Error) {
        toast({
          title: 'Falha ao atualizar empresa',
          description: error.message,
        })

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
                <CardTitle>Editar informações da empresa <Badge className="m-2" variant={"default"}>{form.getValues("trade_name") || "... carregando"}</Badge></CardTitle>
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
                                  ref={maskedCepRef}
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
                                {isPostalCodeLoading || !field.value || isPostalCodeFetching
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
                                {isPostalCodeLoading || !field.value || isPostalCodeFetching
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
                                {isPostalCodeLoading || !field.value || isPostalCodeFetching
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
                                {isPostalCodeLoading || !field.value || isPostalCodeFetching
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
                  <ButtonSuccess isLoading={isLoadingUpdateCompany} isSuccess={updateSuccess} defaultText="Atualizar" loadingText="Atualizando..." successText="Sucesso !" />
                </div>
              </CardContent>
            </Card>
          </form>
        </Form>

      </div>
    </>
  )
}