import { createApi } from "@reduxjs/toolkit/query/react";
import { customBaseQuery } from "./base-api";
import { RequestReponse } from "@/types/request";

export const requestApi = createApi({
  reducerPath: "requestApi",
  baseQuery: customBaseQuery(),
  tagTypes: ["Requests"],
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

    getMyRequests: builder.query<RequestReponse, void>({
      query: () => ({
        url: "request/myself",
        method: 'POST',
        body: {}
      }),
      providesTags: ["Requests"]
    }),


  })
});

export const { useRegisterRequestMutation, useGetMyRequestsQuery } = requestApi;