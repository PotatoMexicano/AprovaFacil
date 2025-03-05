import { createApi, fakeBaseQuery } from "@reduxjs/toolkit/query/react";
import { User } from "../features/users/User";

const fakeUsers: User[] = [
  {
    id: 1,
    nome: "João Souza",
    cargo: "Basico",
    setor: "Engenharia",
    urlPicture: "https://avatar.iran.liara.run/public/5"
  },
  {
    id: 2,
    nome: "Felipe Nunes",
    cargo: "Gerente",
    setor: "Engenharia",
    urlPicture: "https://avatar.iran.liara.run/public/43"
  },
  {
    id: 3,
    nome: "Manoel Silva",
    cargo: "Financeiro",
    setor: "Financeiro",
    urlPicture: "https://avatar.iran.liara.run/public/29" 
  },
  {
    id: 4,
    nome: "Gustavo Lopes",
    cargo: "Diretor",
    setor: "Recursos Humanos",
    urlPicture: "https://avatar.iran.liara.run/public/46"
  },
  {
    id: 5,
    nome: "Leonardo Gouvea",
    cargo: "Diretor",
    setor: "Engenharia",
    urlPicture: "https://avatar.iran.liara.run/public/41"
  },
]

export const userApi = createApi({
  reducerPath: "userApi",
  baseQuery: fakeBaseQuery(),
  endpoints: (builder) => ({
    getUsers: builder.query<User[], void>({
      queryFn: async () => {
        return new Promise((resolve, reject) => {
          setTimeout(() => {
            if (Math.random() < 0) {
              reject(new Error("Falha ao buscar os dados"));
            } else {
              resolve({data: fakeUsers});
            }
          }, 1500);
        })
      }
    })
  })
});

export const { useGetUsersQuery } = userApi;