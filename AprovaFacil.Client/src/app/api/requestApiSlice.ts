import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { Request } from "../features/requests/view-requests/columns";

export const requestApi = createApi({
  reducerPath: "requestApi",
  baseQuery: fetchBaseQuery({ baseUrl: "https://localhost:7296/api/request" }),
  tagTypes: ["Requests"],
  endpoints: (builder) => ({
    registerRequest: builder.mutation<Request, Request>({
      query: (requestData) => {
        const formData = new FormData();

        formData.append("managerId", requestData.managerId.toString());
        formData.append("companyId", requestData.companyId.toString());
        requestData.directorsIds.forEach((id, index) => {
          formData.append(`DirectorsIds[${index}]`, id.toString());
        });
        formData.append("paymentDate", requestData.paymentDate.toISOString());
        formData.append("amount", requestData.amount.toString());
        if (requestData.note) formData.append("note", requestData.note);
        if (requestData.invoice) formData.append("invoice", requestData.invoice);
        if (requestData.budget) formData.append("budget", requestData.budget);

        return {
          url: `register`,
          method: `POST`,
          body: formData,
          // Não definimos headers aqui, o navegador definirá automaticamente Content-Type como multipart/form-data
        };
      },
      invalidatesTags: ["Requests"]
    }),


  })
});

export const { useRegisterRequestMutation } = requestApi;