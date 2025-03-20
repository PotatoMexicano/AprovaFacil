import { createApi } from "@reduxjs/toolkit/query/react";
import { LoginRequest, UserResponse } from "@/types/auth";
import { customBaseQuery } from "./base-api";

export const authApi = createApi({
  reducerPath: "authApi",
  baseQuery: customBaseQuery(),
  endpoints: (builder) => ({
    login: builder.mutation({
      query: (credentials: LoginRequest) => ({
        url: "auth/login",
        method: "POST",
        body: credentials,
      }),
    }),
    logout: builder.mutation<void, void>({
      query: () => ({
        url: "auth/logout",
        method: "POST"
      }),
    }),
    getCurrentUser: builder.query<{ user: UserResponse }, void>({
      query: () => 'auth/me',
    }),
  }),
});

export const { useLoginMutation, useLogoutMutation, useGetCurrentUserQuery } = authApi;