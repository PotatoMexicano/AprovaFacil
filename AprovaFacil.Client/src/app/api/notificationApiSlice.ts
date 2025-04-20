import { createApi } from "@reduxjs/toolkit/query/react";
import { UserNotification } from "@/types/auth";
import { customBaseQuery } from "./base-api";

export const notificationApi = createApi({
  reducerPath: "notificationApi",
  baseQuery: customBaseQuery(),
  tagTypes: ["Notifications"],
  endpoints: (builder) => ({

    markAsRead: builder.mutation<UserNotification, string>({
      query: (notificationIdentifier: string) => ({
        url: `notification/${notificationIdentifier}/read`,
        method: "POST"
      }),
      invalidatesTags: ["Notifications"]
    }),
    getNotifications: builder.query<UserNotification[], void>({
      query: () => ({
        url: `notification`,
        method: 'GET'
      }),
      providesTags: ["Notifications"]
    })
  }),
});

export const {
  useGetNotificationsQuery,
  useMarkAsReadMutation
} = notificationApi;