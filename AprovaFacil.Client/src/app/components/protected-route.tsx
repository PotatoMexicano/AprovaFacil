import { Navigate, Outlet } from "react-router-dom"
import { RootState, useAppSelector } from "../store/store"
import {jwtDecode} from 'jwt-decode';


const isTokenValid = (token: string | null): boolean => {
  if (!token) return false;

  try {
    const decoded: any = jwtDecode(token);
    const currentTime = Date.now() / 1000; // Hora atual em segundos
    return decoded.exp && decoded.exp > currentTime; // Verifica se o token não expirou
  } catch (error) {
    return false;
  }
}

const ProtectedRoute: React.FC = () => {
  const { isAuthenticated, token } = useAppSelector((state: RootState) => state.auth)

  if (!isAuthenticated || !isTokenValid(token)) {
    return <Navigate to="/login" replace />
  }

  return <Outlet />
};

export default ProtectedRoute;