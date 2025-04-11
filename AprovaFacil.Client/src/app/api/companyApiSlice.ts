import { createApi } from "@reduxjs/toolkit/query/react";
import { CompanyResponse } from "@/types/company";
import { customBaseQuery } from "./base-api";

export const companyApi = createApi({
  reducerPath: "companyApi",
  baseQuery: customBaseQuery(),
  tagTypes: ["Companies"],
  endpoints: (builder) => ({
    getCompanies: builder.query<CompanyResponse[], void>({
      query: () => 'company',
      providesTags: ["Companies"]
    }),

    getCompany: builder.query<CompanyResponse | undefined, string>({
      query: (id) => `company/${id}`
    }),

    registerCompany: (builder.mutation<CompanyResponse, CompanyResponse>({
      query: (company) => ({
        url: `company/register`,
        method: `POST`,
        body: company,
      }),
      invalidatesTags: ["Companies"]
    })),

    updateCompany: (builder.mutation<CompanyResponse, CompanyResponse>({
      query: (company) => ({
        url: `company/update`,
        method: `POST`,
        body: company
      }),
      invalidatesTags: ["Companies"]
    })),

    removeCompany: (builder.mutation<void, number>({
      query: (id) => ({
        url: `company/${id}`,
        method: `DELETE`,
      }),
      invalidatesTags: ["Companies"]
    })),
  })
});

export const { useGetCompaniesQuery, useGetCompanyQuery, useUpdateCompanyMutation, useRegisterCompanyMutation, useRemoveCompanyMutation } = companyApi;