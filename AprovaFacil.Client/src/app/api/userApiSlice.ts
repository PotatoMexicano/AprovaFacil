import { UserResponse } from "@/types/auth";
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

export const userApi = createApi({
  reducerPath: "userApi",
  baseQuery: fetchBaseQuery({ baseUrl: "https://localhost:7296/api/user" }),
  tagTypes: ["Users"],
  endpoints: (builder) => ({
    getUsers: builder.query<UserResponse[], void>({
      query: () => '',
      providesTags: ["Users"]
    }),

    getUser: builder.query<UserResponse | undefined, number>({
      query: (idUser) => `/${idUser}`
    }),
  })
});

export const { useGetUsersQuery, useGetUserQuery } = userApi;