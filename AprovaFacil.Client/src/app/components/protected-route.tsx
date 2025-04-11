import { Outlet, useNavigate } from "react-router-dom"
import { authApi, useGetCurrentUserQuery } from "../api/authApiSlice";
import { clearUser } from "@/auth/authSlice";
import { useDispatch } from "react-redux";
import { useEffect } from "react";

const ProtectedRoute: React.FC = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const { data, isLoading, isError, error} = useGetCurrentUserQuery();

  useEffect(() => {
    if (!isLoading) {
      if (isError || !data) {
        dispatch(clearUser());
        dispatch(authApi.util.resetApiState());
        window.location.replace('/login')
        return;
      }
    }
  }, [isLoading, isError, data, error, navigate, dispatch]);
  
  if (isLoading) {
    return <div></div>; // Evita renderizar App enquanto carrega
  }
  
  if (isError) {
    return <div>Erro: {(error as any)?.message || 'Falha na conexão'}</div>;
  }
  
  return <Outlet />; // Só renderiza App se autenticado
};

export default ProtectedRoute;