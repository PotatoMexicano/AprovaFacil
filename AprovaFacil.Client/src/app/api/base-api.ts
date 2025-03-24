import { fetchBaseQuery } from "@reduxjs/toolkit/query"
// import { RootState } from "../store/store";

export const customBaseQuery = () => {
  return fetchBaseQuery({
    baseUrl: `${import.meta.env.VITE_API_URL}/api`, // Base URL da sua API
    credentials: 'include',
    // prepareHeaders: (headers, { getState }) => {
    //   // const token = (getState() as RootState).auth.token; // Pega o token do estado global (Redux)
    //   // if (token) {
    //   //   // Adiciona o token nas requisições
    //   //   headers.set('Authorization', `Bearer ${token}`);
    //   // }

    //   return headers;
    // },
  });
}