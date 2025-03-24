import { UserResponse } from "@/types/auth";
import { createApi } from "@reduxjs/toolkit/query/react";
import { customBaseQuery } from "./base-api";

export const userApi = createApi({
  reducerPath: "userApi",
  baseQuery: customBaseQuery(),
  tagTypes: ["Users", "Enabled"],
  endpoints: (builder) => ({
    getUsers: builder.query<UserResponse[], void>({
      query: () => 'user',
      providesTags: ["Users"]
    }),

    getEnabledUsers: builder.query<UserResponse[], void>({
      query: () => 'user/enabled',
      providesTags: ['Enabled']
    }),

    getUser: builder.query<UserResponse | undefined, string>({
      query: (idUser) => `user/${idUser}`
    }),

    disableUser: builder.mutation<boolean, number>({
      query: (idUser) => ({
        url: `user/${idUser}/disable`,
        method: `POST`,
      }),
      invalidatesTags: ["Users", "Enabled"]
    }),

    enableUser: builder.mutation<boolean, number>({
      query: (idUser) => ({
        url: `user/${idUser}/enable`,
        method: `POST`,
      }),
      invalidatesTags: ["Users", "Enabled"]
    }),

    registerUser: builder.mutation<UserResponse, Omit<UserResponse, "id" | "enabled" | "identity_roles">>({
      query: (userData) => ({
        url: `user/register`,
        method: `POST`,
        body: userData,
      }),
      invalidatesTags: ["Users", "Enabled"],
    }),

  })
});

export const {
  useGetUsersQuery,
  useGetUserQuery,
  useGetEnabledUsersQuery,
  useDisableUserMutation,
  useEnableUserMutation,
  useRegisterUserMutation } = userApi;