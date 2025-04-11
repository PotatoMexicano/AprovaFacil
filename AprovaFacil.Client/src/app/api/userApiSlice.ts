import { UserResponse } from "@/types/auth";
import { createApi } from "@reduxjs/toolkit/query/react";
import { customBaseQuery } from "./base-api";

export const userApi = createApi({
  reducerPath: "userApi",
  baseQuery: customBaseQuery(),
  tagTypes: ["Users"],
  endpoints: (builder) => ({
    getUsers: builder.query<UserResponse[], void>({
      query: () => 'user',
      providesTags: ["Users"]
    }),

    getEnabledUsers: builder.query<UserResponse[], void>({
      query: () => 'user/enabled',
      providesTags: ["Users"]
    }),

    getUser: builder.query<UserResponse | undefined, string>({
      query: (idUser) => `user/${idUser}`,
      providesTags: ["Users"]
    }),

    disableUser: builder.mutation<boolean, number>({
      query: (idUser) => ({
        url: `user/${idUser}/disable`,
        method: `POST`,
      }),
      invalidatesTags: ["Users"]
    }),

    enableUser: builder.mutation<boolean, number>({
      query: (idUser) => ({
        url: `user/${idUser}/enable`,
        method: `POST`,
      }),
      invalidatesTags: ["Users"]
    }),

    registerUser: builder.mutation<UserResponse, Omit<UserResponse, "id" | "enabled" | "identity_roles">>({
      query: (userData) => ({
        url: `user/register`,
        method: `POST`,
        body: userData,
      }),
      invalidatesTags: ["Users"],
    }),

    updateUser: builder.mutation<UserResponse, Omit<UserResponse, "role_label" | "department_label" | "enabled" | "identity_roles">>({
      query: (userData) => ({
        url: 'user/update',
        method: 'POST',
        body: userData,
      }),
      invalidatesTags: ["Users"],
    })

  })
});

export const {
  useGetUsersQuery,
  useGetUserQuery,
  useGetEnabledUsersQuery,
  useDisableUserMutation,
  useEnableUserMutation,
  useRegisterUserMutation,
  useUpdateUserMutation } = userApi;