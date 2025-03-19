"use client"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/app/components/ui/card"
import { Input } from "@/app/components/ui/input"
import { useBreadcrumb } from "@/app/context/breadkcrumb-context"
import { useEffect, useState } from "react"
import type { z } from "zod"
import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/app/components/ui/form"
import { toast } from "@/hooks/use-toast"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/app/components/ui/select"
import ButtonSuccess from "@/app/components/ui/button-success"
import userSchema from "@/app/schemas/userSchema"
import { useRegisterUserMutation } from "@/app/api/userApiSlice"
import { useNavigate } from "react-router-dom"

export default function NewUserPage() {
  const { setBreadcrumbs } = useBreadcrumb()
  const navigate = useNavigate()

  const [registerUser, { isLoading, isSuccess, isError }] = useRegisterUserMutation()
  const [registerSuccess, setRegisterSuccess] = useState<boolean | undefined>(undefined)

  const form = useForm<z.infer<typeof userSchema>>({
    resolver: zodResolver(userSchema),
    defaultValues: {
      full_name: "",
      role: "",
      department: "",
      picture_url: "",
      email: "",
      password: "",
    },
  })

  // Predefined roles and departments based on your application
  const roles = [
    { value: "Manager", label: "Gerente" },
    { value: "Director", label: "Diretor" },
    { value: "Analyst", label: "Analista" },
    { value: "Assistant", label: "Assistente" },
  ]

  const departments = [
    { value: "Finance", label: "Financeiro" },
    { value: "HR", label: "Recursos Humanos" },
    { value: "IT", label: "Tecnologia da Informação" },
    { value: "Marketing", label: "Marketing" },
    { value: "Operations", label: "Operações" },
    { value: "Sales", label: "Vendas" },
  ]

  useEffect(() => {
    setRegisterSuccess(isSuccess)
    if (isSuccess) {
      toast({
        title: "Usuário registrado",
        description: "O usuário foi registrado com sucesso.",
      })

      // Redirect to users list after successful registration
      setTimeout(() => {
        navigate("/users")
      }, 2000)
    }
    if (isError) {
      toast({
        title: "Falha ao registrar usuário",
        description: "Não foi possível registrar o usuário.",
      })
    }

    const timer = setTimeout(() => {
      setRegisterSuccess(undefined)
    }, 3500)

    return () => clearTimeout(timer)
  }, [isSuccess, isError, navigate])

  useEffect(() => {
    setBreadcrumbs(["Início", "Usuários", "Adicionar"])
  }, [setBreadcrumbs])

  async function onSubmit(values: z.infer<typeof userSchema>) {
    try {
      await registerUser(values).unwrap()
    } catch (error) {
      console.error(`Erro ao registrar usuário:`, error)

      if (error instanceof Error) {
        toast({
          title: "Falha ao registrar usuário",
          description: error.message,
        })
      }
    }
  }

  return (
    <>
      <Card className="col-span-12 flex flex-col shadow-none border-0">
        <CardHeader>
          <CardTitle>Cadastro de Usuário</CardTitle>
          <CardDescription>Preencha os dados para cadastrar um novo usuário no sistema.</CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-4">
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
                          <Input placeholder="joao.silva@exemplo.com" type="email" {...field} />
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
                          <Input placeholder="******" type="password" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="picture_url"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>URL da Imagem de Perfil</FormLabel>
                        <FormControl>
                          <Input placeholder="https://exemplo.com/imagem.jpg" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <div className="space-y-4">
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

                  {form.getValues("picture_url") && (
                    <div className="mt-4">
                      <p className="text-sm font-medium mb-2">Pré-visualização da imagem:</p>
                      <div className="border rounded-lg p-2 flex justify-center">
                        <img
                          src={form.getValues("picture_url") || "/placeholder.svg?height=100&width=100"}
                          alt="Pré-visualização"
                          className="h-24 w-24 rounded-full object-cover"
                          onError={(e) => {
                            e.currentTarget.src = "/placeholder.svg?height=100&width=100"
                          }}
                        />
                      </div>
                    </div>
                  )}
                </div>
              </div>

              <div className="pt-4">
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
      </Card>
    </>
  )
}

