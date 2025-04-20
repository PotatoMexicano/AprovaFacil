import { Outlet, useNavigate } from "react-router-dom"
import { useLazyGetCurrentUserQuery } from "../api/authApiSlice";
import { clearUser } from "@/auth/authSlice";
import { useDispatch } from "react-redux";
// import { jwtDecode } from 'jwt-decode'
import { useEffect, useState } from "react";
import { useIsAdmin, useIsFinance } from "@/lib/utils";
import { toast } from "sonner";

const FinanceRoute: React.FC = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const isFinance = useIsFinance();

  const [trigger, { data, isLoading, isError, error, isFetching }] = useLazyGetCurrentUserQuery();
  const [hasTriggered, setHasTriggered] = useState(false);

  useEffect(() => {
      if (!isFinance && data && !isLoading) {
        toast.error("Permissoes insuficientes.")
        navigate("/", { replace: true});
        return;
    }
  }, [data, isLoading, isFinance, navigate]);

  useEffect(() => {
    // Dispara a requisição manualmente após um pequeno atraso
    const timer = setTimeout(() => {
      trigger(undefined, true); // Dispara a requisição
      setHasTriggered(true);
    }, 100); // Atraso de 100ms para garantir que o cookie esteja disponível

    return () => clearTimeout(timer);
  }, [trigger]);

  useEffect(() => {
    if (!hasTriggered) return; // Só executa após a requisição ser disparada
    if (!isLoading && !isFetching) {
      if (isError || !data) {
        navigate('/login', { replace: true });
        dispatch(clearUser());
      }
    }
  }, [isLoading, isFetching, isError, data, error, hasTriggered, navigate, dispatch]);

  if (isLoading || !data || !isFinance) {
    return <div></div>; // Evita renderizar App enquanto carrega
  }

  if (isError) {
    return <div>Erro: {(error as any)?.message || 'Falha na conexão'}</div>;
  }

  return <Outlet />; // Só renderiza App se autenticado
};

export default FinanceRoute;