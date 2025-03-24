import { UserResponse } from "@/types/auth";
import { createSlice} from "@reduxjs/toolkit";

// src/features/auth/authSlice.ts
interface AuthState {
  //token: string | null,
  user: UserResponse | null;
  isAuthenticated: boolean;
}

const initialState: AuthState = {
  //token: null,
  user: null,
  isAuthenticated: false,
};

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setUser: (state, action) => {
      state.user = action.payload.user;
      //state.token = action.payload.token;
      state.isAuthenticated = true;
    },
    clearUser: (state) => {
      //state.token = null;
      state.user = null;
      state.isAuthenticated = false;
    },
    disableUser: (state) => {
      state.isAuthenticated = false
    }
  },
});

export const { setUser, clearUser, disableUser } = authSlice.actions;
export default authSlice.reducer;

export const saveAuthSate = (state: AuthState) => {
  localStorage.setItem('authState', JSON.stringify(state));
}

export const loadAuthState = (): AuthState => {
  const savedState = localStorage.getItem('authState');
  return savedState ? JSON.parse(savedState) : initialState;
}