import { createApi } from "@reduxjs/toolkit/query/react";
import { LoginRequest, LoginResponse, UserResponse } from "@/types/auth";
import { customBaseQuery } from "./base-api";

export const authApi = createApi({
  reducerPath: "authApi",
  baseQuery: customBaseQuery(),
  endpoints: (builder) => ({
    login: builder.mutation<LoginResponse, LoginRequest>({
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
    getCurrentUser: builder.query<UserResponse, void>({
      query: () => ({
        url: `auth/me`,
        method: 'GET'
      }),
    })
  }),
});

export const {
  useLoginMutation,
  useLogoutMutation,
  useGetCurrentUserQuery,
  useLazyGetCurrentUserQuery
} = authApi;