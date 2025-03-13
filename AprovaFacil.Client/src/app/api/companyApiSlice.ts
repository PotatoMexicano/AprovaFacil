import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { Company } from "@/types/company";

export const companyApi = createApi({
  reducerPath: "companyApi",
  baseQuery: fetchBaseQuery({ baseUrl: "https://localhost:7296/api/company" }),
  tagTypes: ["Companies"],
  endpoints: (builder) => ({
    getCompanies: builder.query<Company[], void>({
      query: () => '',
      providesTags: ["Companies"]
    }),

    getCompany: builder.query<Company | undefined, string>({
      query: (id) => `/${id}`
    }),

    registerCompany: (builder.mutation<Company, Company>({
      query: (company) => ({
        url: `register`,
        method: `POST`,
        body: company,
      }),
      invalidatesTags: ["Companies"]
    })),

    updateCompany: (builder.mutation<Company, Company>({
      query: (company) => ({
        url: `update`,
        method: `POST`,
        body: company
      }),
      invalidatesTags: ["Companies"]
    })),

    removeCompany: (builder.mutation<void, number>({
      query: (id) => ({
        url: `/${id}`,
        method: `DELETE`,
      }),
      invalidatesTags: ["Companies"]
    })),
  })
});

export const { useGetCompaniesQuery, useGetCompanyQuery, useUpdateCompanyMutation, useRegisterCompanyMutation, useRemoveCompanyMutation } = companyApi;