import { Navigate, Outlet } from "react-router-dom"
import { RootState, useAppSelector } from "../store/store"

const ProtectedRoute: React.FC = () => {
  const { isAuthenticated } = useAppSelector((state: RootState) => state.auth)
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  return <Outlet />
};

export default ProtectedRoute;