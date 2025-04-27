import { createApi } from "@reduxjs/toolkit/query/react";
import { customBaseQuery } from "./base-api";
import { RequestReponse, RequestStatsResponse } from "@/types/request";

export const requestApi = createApi({
  reducerPath: "requestApi",
  baseQuery: customBaseQuery(),
  tagTypes: ["Requests", "Approved"], 
  endpoints: (builder) => ({
    registerRequest: builder.mutation({
      query: (requestData) => {
        const formData = new FormData();
        formData.append("companyId", requestData.companyId.toString());
        formData.append("managersId", requestData.managerId.toString());
        requestData.directorsIds.forEach((id, index) => {
          formData.append(`DirectorsIds[${index}]`, id.toString());
        });
        formData.append("paymentDate", requestData.paymentDate.toISOString());
        formData.append("amount", requestData.amount.toString());
        if (requestData.note) formData.append("note", requestData.note);
        if (requestData.invoice) formData.append("invoice", requestData.invoice);
        if (requestData.budget) formData.append("budget", requestData.budget);

        return {
          url: `request/register`,
          method: `POST`,
          body: formData,
        };
      },
      invalidatesTags: ["Requests"]
    }),

    allRequests: builder.query<RequestReponse[], void>({
      query: () => {
        return {
          url: "request",
          method: "GET",
        }
      },
      providesTags: ["Requests"]
    }),

    approveRequest: builder.mutation({
      query: (id) => {
        return {
          url: `request/${id}/approve`,
          method: 'POST',
          priority: 'high'
        }
      },
      invalidatesTags: ["Requests"]
    }),

    rejectRequest: builder.mutation({
      query: (id) => {
        return {
          url: `request/${id}/reject`,
          method: 'POST',
          priority: 'high'
        }
      },
      invalidatesTags: ["Requests"]
    }),

    finishRequest: builder.mutation({
      query: (id) => {
        return {
          url: `request/${id}/finish`,
          method: `POST`,
          priority: `high`
        }
      },
      invalidatesTags: ["Requests"]
    }),

    getRequest: builder.query<RequestReponse, string>({
      query: (id) => ({
        url: `request/${id}`,
        method: "GET",
      }),
      providesTags: ["Requests"]
    }),

    getMyRequests: builder.query<RequestReponse[], number>({
      query: (quantity?: number) => ({
        url: "request/myself",
        method: 'POST',
        body: {
          quantity: quantity
        }
      }),
      providesTags: ["Requests"]
    }),

    getMyStats: builder.query<RequestStatsResponse, void>({
      query: () => ({
        url: "request/myself/stats",
        method: 'GET',
      }),
    }),

    getPendingRequests: builder.query<RequestReponse[], void>({
      query: () => ({
        url: "request/pending",
        method: "POST",
        body: {}
      }),
      providesTags: ["Requests"]
    }),

    getApprovedRequests: builder.query<RequestReponse[], void>({
      query: () => ({
        url: "request/approved",
        method: "POST",
      }),
      providesTags: ["Approved"],
    }),

    getFinishedRequests: builder.query<RequestStatsResponse[], void>({
      query: () => ({
        url: "request/finished",
        method: "POST",
      }),
      providesTags: ["Requests"],
    }),

    getFileRequest: builder.query<{ blob: Blob, fileName: string }, { fileType: string, requestId: string, fileId: string }>({
      query: ({ fileType, requestId, fileId }) => ({
        url: `request/file/${fileType}/${requestId}/${fileId}`,
        method: "GET",
        responseHandler: async (response) => {

          if (!response.ok) {
            throw new Error(`Erro na requisição: ${response.statusText}`);
          }

          // Extrai o header Content-Disposition
          const contentDisposition = response.headers.get("content-disposition");
          let fileName = "default_filename.pdf"; // Nome padrão caso não encontre

          if (contentDisposition) {
            let match = contentDisposition.match(/filename="(.+?)"/);
            if (match && match[1]) {
              fileName = match[1];
            } else {
              match = contentDisposition.match(/filename\*=UTF-8''(.+)/);
              if (match && match[1]) {
                fileName = decodeURIComponent(match[1]);
              }
            }

            // Removendo possíveis aspas extras
            fileName = fileName.replace(/['"]/g, "");
          }

          // Converte o corpo da resposta para Blob
          const blob = await response.blob();

          // Retorna um objeto com o Blob e o nome do arquivo
          return { blob, fileName };
        },
      })
    }),
  })
});

export const {
  useRegisterRequestMutation,
  useGetMyRequestsQuery,
  useGetPendingRequestsQuery,
  useGetFinishedRequestsQuery,
  useLazyGetPendingRequestsQuery,
  useLazyGetFileRequestQuery,
  useGetRequestQuery,
  useApproveRequestMutation,
  useRejectRequestMutation,
  useFinishRequestMutation,
  useAllRequestsQuery,
  useGetMyStatsQuery,
  useGetApprovedRequestsQuery,
} = requestApi;