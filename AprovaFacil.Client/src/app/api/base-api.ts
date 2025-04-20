import { fetchBaseQuery } from "@reduxjs/toolkit/query"
// import { RootState } from "../store/store";

export const customBaseQuery = () => {
  return fetchBaseQuery({
    baseUrl: `${import.meta.env.VITE_API_URL}/api`,
    credentials: 'include',
  });
}