import { configureStore } from '@reduxjs/toolkit'
import { useDispatch, useSelector } from 'react-redux';
import { cepApi } from '../api/cepApiSlice';
import { companyApi } from '../api/companyApiSlice';
import { authApi } from '../api/authApiSlice';
import { requestApi } from '../api/requestApiSlice';
import authReducer, {loadAuthState, saveAuthSate} from '../../auth/authSlice';
import { userApi } from '../api/userApiSlice';

export const store = configureStore({
  reducer: {
    [cepApi.reducerPath]: cepApi.reducer,
    [companyApi.reducerPath]: companyApi.reducer,
    [authApi.reducerPath]: authApi.reducer,
    [requestApi.reducerPath]: requestApi.reducer,
    [userApi.reducerPath]: userApi.reducer,
    auth: authReducer,
  },
  preloadedState: {
    auth: loadAuthState(),
  },
  middleware: (getDefaultmiddleware) => getDefaultmiddleware()
  .concat(cepApi.middleware, companyApi.middleware, authApi.middleware, requestApi.middleware, userApi.middleware)
});

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch

export const useAppDispatch = useDispatch.withTypes<AppDispatch>()
export const useAppSelector = useSelector.withTypes<RootState>()

store.subscribe(() => {
  saveAuthSate(store.getState().auth);
});

export default store;