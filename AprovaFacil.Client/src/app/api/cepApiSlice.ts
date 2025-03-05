import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

export const cepApi = createApi({
  reducerPath: 'cepApi',
  baseQuery: fetchBaseQuery({ baseUrl: 'https://viacep.com.br/ws/' }),
  endpoints: (builder) => ({
    getCep: builder.query({
      query: (cep) => `${cep}/json/`,
      // Adicionando um onErrorHandler para capturar os erros
      transformResponse: async (response, meta) => {
        return new Promise((resolve, reject) => {
          if (response.erro) {
            reject(new Error('CEP inválido ou não encontrado.'));
          }
          else {
            resolve(response);
          }
        })
      },
      onError: (error, { rejectWithValue }) => {
        if (error instanceof Error) {
          // Se for erro de rede ou algo do tipo, trata o erro
          return rejectWithValue({ message: 'Erro ao conectar com a API. Verifique sua conexão.' });
        }
        return rejectWithValue({ message: 'Ocorreu um erro desconhecido.' });
      },
    }),
  }),
});

export const { useGetCepQuery } = cepApi;