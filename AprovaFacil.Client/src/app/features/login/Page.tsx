import { cn } from "@/lib/utils"
import { Button } from "@/app/components/ui/button"
import { Card, CardContent } from "@/app/components/ui/card"
import { Input } from "@/app/components/ui/input"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/app/components/ui/form"
import formSchema from "@/app/schemas/authSchema";
import { z } from "zod"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { authApi, useLazyGetCurrentUserQuery, useLoginMutation } from "@/app/api/authApiSlice"
import { useDispatch } from "react-redux"
import { setUser } from "@/auth/authSlice"
import { toast, Toaster } from "sonner"
import { useNavigate } from "react-router-dom"
import { ThemeProvider } from "@/app/components/theme-provider"

export default function LoginForm({ className, ...props }: React.ComponentProps<"div">) {

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const [login, { isLoading: isLoadingAuth, isSuccess }] = useLoginMutation();
  const [getUser] = useLazyGetCurrentUserQuery();


  const dispatch = useDispatch();
  const navigate = useNavigate();

  async function onSubmit(values: z.infer<typeof formSchema>) {
    try {
      const response = await login({ email: values.email, password: values.password }).unwrap();

      dispatch(setUser(response));
      dispatch(authApi.util.resetApiState());

      const userData = await getUser().unwrap();

      if (userData) {
        // Usuário já está autenticado, redireciona para a página principal
        navigate('/', { replace: false });
      }

    } catch (error) {
      const errorMessage =
        error?.data?.message || error?.message || 'Erro ao fazer login. Verifique suas credenciais.';
      toast.error(`Erro no login: ${JSON.stringify(errorMessage)}`);
    }
  }

  return (
    <div className="flex min-h-svh flex-col items-center justify-center bg-muted p-6 md:p-10">
      <ThemeProvider defaultTheme="system" storageKey="vite-ui-theme">
        <Toaster expand={false} />
        <div className="w-full max-w-sm md:max-w-3xl">
          <div className={cn("flex flex-col gap-6", className)} {...props}>
            <Card className="overflow-hidden">
              <CardContent className="grid p-0 md:grid-cols-2">
                <Form {...form}>
                  <form className="p-6 md:p-8" onSubmit={form.handleSubmit(onSubmit)}>
                    <div className="flex flex-col gap-6">
                      <div className="flex flex-col items-center text-center">
                        <h1 className="text-2xl font-bold">Bem - Vindo</h1>
                        <p className="text-balance text-muted-foreground">
                          Entre na sua conta AprovaFácil
                        </p>
                      </div>
                      <div className="grid gap-2">
                        <FormField
                          control={form.control}
                          name="email"
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>Email</FormLabel>
                              <FormControl>
                                <Input {...field} type="email" placeholder="paulo@example.com" required />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />

                      </div>
                      <div className="grid gap-2">
                        <FormField
                          control={form.control}
                          name="password"
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>
                                <div className="flex justify-between">
                                  Senha
                                  <a href="#" className="ml-auto text-sm font-light underline-offset-2 hover:underline">Esqueceu a senha ?</a>
                                </div>
                              </FormLabel>
                              <FormControl>
                                <Input {...field} type="password" placeholder="******" required />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                      </div>
                      <Button type="submit" className="w-full" disabled={isLoadingAuth}>
                        {isLoadingAuth ? 'Entrando...' : 'Entrar'}
                      </Button>
                    </div>
                  </form>
                </Form>
                <div className="relative hidden bg-muted md:block">
                  <a href="https://storyset.com/nature" target="_blank" className="flex justify-center">
                    <div className="relative text-center">
                      <img
                        src="/strelitzia plant-amico.svg"
                        alt="Image"
                        className="inset-0 h-full w-full object-cover -z-1"
                      />
                      <small className="p-1 text-gray-400 z-10 bottom-0">Nature illustrations by Storyset</small>
                    </div>
                  </a>
                </div>
              </CardContent>
            </Card>
            <div className="text-balance text-center text-xs text-muted-foreground [&_a]:underline [&_a]:underline-offset-4 hover:[&_a]:text-primary">
              By clicking continue, you agree to our <a href="#">Terms of Service</a>{" "}
              and <a href="#">Privacy Policy</a>.
            </div>
          </div>
        </div>
      </ThemeProvider>
    </div>
  )
}
