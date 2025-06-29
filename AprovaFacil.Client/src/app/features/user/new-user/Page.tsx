"use client"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card"
import { Input } from "@/app/components/ui/input"
import { useBreadcrumb } from "@/app/context/breadcrumb-context"
import { useEffect, useState } from "react"
import type { z } from "zod"
import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/app/components/ui/form"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/app/components/ui/select"
import ButtonSuccess from "@/app/components/ui/button-success"
import userSchema from "@/app/schemas/userSchema"
import { useRegisterUserMutation } from "@/app/api/userApiSlice"
import { useNavigate } from "react-router-dom"
import AvatarCarousel from "@/app/components/ui/avatar-carousel"
import { toast } from "sonner"

export default function NewUserPage() {
  const { setBreadcrumbs } = useBreadcrumb()
  const navigate = useNavigate()

  const [registerUser, { isLoading, isSuccess, isError, error: errorRegisterUser }] = useRegisterUserMutation()
  const [registerSuccess, setRegisterSuccess] = useState<boolean | undefined>(undefined)

  const form = useForm<z.infer<typeof userSchema>>({
    resolver: zodResolver(userSchema),
    defaultValues: {
      full_name: "",
      role: "",
      department: "",
      picture_url: '/avatars/female/82.png',
      email: "",
      password: "",
    },
  })

  // Predefined roles and departments based on your application
  const roles = [
    { value: "Assistant", label: "Assistente" },
    { value: "Finance", label: "Analista Financeiro" },
    { value: "Director", label: "Diretor" },
    { value: "Manager", label: "Gerente" },
    { value: "Requester", label: "Requisitante" },
  ]

  const departments = [
    { value: "Finance", label: "Financeiro" },
    { value: "HR", label: "Recursos Humanos" },
    { value: "IT", label: "Tecnologia da Informação" },
    { value: "Marketing", label: "Marketing" },
    { value: "Operations", label: "Operações" },
    { value: "Sales", label: "Vendas" },
    { value: "Engineer", label: "Engenharia" },
  ]

  useEffect(() => {
    setRegisterSuccess(isSuccess)

    const timer = setTimeout(() => {
      setRegisterSuccess(undefined)
    }, 3500)

    return () => clearTimeout(timer)
  }, [isSuccess, isError, navigate]);

  useEffect(() => {
    setBreadcrumbs(["Início", "Usuários", "Adicionar"])
  }, [setBreadcrumbs])

  async function onSubmit(values: z.infer<typeof userSchema>) {
    try {
      await registerUser(values).unwrap();

      setRegisterSuccess(isSuccess);

      toast.success("O usuário foi registrado com sucesso.");

      // Redirect to users list after successful registration
      setTimeout(() => {
        navigate("/users")
      }, 2000)

    } catch (error: unknown) {
      console.error("Erro ao registrar usuário:", error);

      const errorMessage = error?.data?.detail || "Erro desconhecido ao registrar usuário";

      toast.error(errorMessage);

    }
  }

  return (
    <Card className="col-span-12 flex flex-col shadow-none border-0">
      <CardHeader>
        <CardTitle>Cadastro de Usuário</CardTitle>
        <CardDescription>Preencha os dados para cadastrar um novo usuário no sistema.</CardDescription>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              
              <div className="flex flex-col space-y-2">
                <FormField
                  control={form.control}
                  name="full_name"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Nome Completo</FormLabel>
                      <FormControl>
                        <Input placeholder="João da Silva" {...field} />
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
                        <Input placeholder="joao.silva@exemplo.com" type="email" {...field} autoComplete="off" />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="password"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Senha</FormLabel>
                      <FormControl>
                        <Input placeholder="******" type="password" {...field} autoComplete="new-password" />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

              </div>

              <div className="flex flex-col space-y-2">
                <FormField
                  control={form.control}
                  name="role"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Cargo</FormLabel>
                      <Select onValueChange={field.onChange} defaultValue={field.value}>
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione um cargo" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {roles.map((role) => (
                            <SelectItem key={role.value} value={role.value}>
                              {role.label}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="department"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Departamento</FormLabel>
                      <Select onValueChange={field.onChange} defaultValue={field.value}>
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione um departamento" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {departments.map((dept) => (
                            <SelectItem key={dept.value} value={dept.value}>
                              {dept.label}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

            </div>

            <div className="flex flex-col space-y-2 items-center">
              <FormField
                control={form.control}
                name="picture_url"
                render={({ field }) => (
                  <FormItem className="text-center">
                    <FormControl>
                      <AvatarCarousel field={field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="col-span-12 justify-start pt-4">
              <ButtonSuccess
                isLoading={isLoading}
                isSuccess={registerSuccess}
                defaultText="Cadastrar Usuário"
                loadingText="Cadastrando..."
                successText="Usuário Cadastrado!"
              />
            </div>
        </form>
      </Form>
    </CardContent>
    </Card >
  )
}

