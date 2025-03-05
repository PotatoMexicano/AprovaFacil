import { configureStore } from '@reduxjs/toolkit'
import { useDispatch, useSelector } from 'react-redux';
import { cepApi } from '../api/cepApiSlice';
import { companyApi } from '../api/companyApiSlice';
import { userApi } from '../api/userApiSlice';
import { requestApi } from '../api/requestApiSlice';
export const store = configureStore({
  reducer: {
    [cepApi.reducerPath]: cepApi.reducer,
    [companyApi.reducerPath]: companyApi.reducer,
    [userApi.reducerPath]: userApi.reducer,
    [requestApi.reducerPath]: requestApi.reducer,
  },
  middleware: (getDefaultmiddleware) => getDefaultmiddleware()
  .concat(cepApi.middleware, companyApi.middleware, userApi.middleware, requestApi.middleware)
});

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch

export const useAppDispatch = useDispatch.withTypes<AppDispatch>()
export const useAppSelector = useSelector.withTypes<RootState>()