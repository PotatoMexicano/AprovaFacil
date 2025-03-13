import { UserResponse } from "@/types/auth";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

// src/features/auth/authSlice.ts
interface AuthState {
  user: UserResponse | null;
  isAuthenticated: boolean;
}

const initialState: AuthState = {
  user: null,
  isAuthenticated: false,
};

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setUser: (state, action: PayloadAction<UserResponse>) => {
      state.user = {
        ...action.payload,
        identity_roles: action.payload.identity_roles || [],
      };
      state.isAuthenticated = true;
    },
    clearUser: (state) => {
      state.user = null;
      state.isAuthenticated = false;
    },
  },
});

export const { setUser, clearUser } = authSlice.actions;
export default authSlice.reducer;

export const saveAuthSate = (state: AuthState) => {
  localStorage.setItem('authState', JSON.stringify(state));
}

export const loadAuthState = (): AuthState => {
  const savedState = localStorage.getItem('authState');
  return savedState ? JSON.parse(savedState) : initialState;
}