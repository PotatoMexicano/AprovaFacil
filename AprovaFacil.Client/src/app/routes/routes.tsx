import { createBrowserRouter, Navigate } from "react-router-dom";
import App from "../layout/App";
import Homepage from "../features/homepage/Homepage";
import ViewCompaniesPage from "../features/company/view-companies/Page";
import NewCompanyPage from "../features/company/new-company/Page";
import EditCompanyPage from "../features/company/edit-company/Page";
import NewRequestPage from "../features/requests/new-request/Page";
import LoginPage from "../features/login/Page";
import ProtectedRoute from "../components/protected-route";
import ViewUsersPage from "../features/user/view-user/Page";
import NewUserPage from "../features/user/new-user/Page";
import EditUserPage from "../features/user/edit-user/Page";
import ViewMyRequestsPage from "../features/requests/view-requests/Page";

export const router = createBrowserRouter([
  { path: 'login', element: <LoginPage /> },
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <App />,
        children: [
          { path: '/', element: <Homepage /> },
          {
            path: 'company', children: [
              { path: 'register', element: <NewCompanyPage /> },
              { path: '', element: <ViewCompaniesPage /> },
              { path: 'edit/:id', element: <EditCompanyPage /> },
            ]
          },
          {
            path: 'request', children: [
              { path: '', element: <ViewMyRequestsPage /> },
              { path: 'register', element: <NewRequestPage /> },
            ]
          },
          {
            path: 'users', children: [
              { path: '', element: <ViewUsersPage /> },
              { path: "register", element: <NewUserPage /> },
              { path: 'edit/:id', element: <EditUserPage /> },
            ]
          }
        ]
      },
      { path: '*', element: <Navigate replace to={'/not-found'} /> }
    ]
  }
])