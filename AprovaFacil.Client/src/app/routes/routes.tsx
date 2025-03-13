import { createBrowserRouter, Navigate } from "react-router-dom";
import App from "../layout/App";
import Homepage from "../features/homepage/Homepage";
import ViewCompaniesPage from "../features/company/view-companies/Page";
import NewCompanyPage from "../features/company/new-company/Page";
import EditCompanyPage from "../features/company/edit-company/Page";
import NewRequestPage from "../features/requests/new-request/Page";
import ViewRequestsPage from "../features/requests/view-requests/Page";
import LoginPage from "../features/login/Page";
import ProtectedRoute from "../components/protected-route";

export const router = createBrowserRouter([
  { path: 'login', element: <LoginPage /> },
  {
    path: '',
    element: <App />,
    children: [
      {
        path: '',
        element: <ProtectedRoute />,
        children: [
          { path: '', element: <Homepage /> },
          {
            path: 'company', children: [
              { path: 'register', element: <NewCompanyPage /> },
              { path: '', element: <ViewCompaniesPage /> },
              { path: 'edit/:id', element: <EditCompanyPage /> },
            ]
          },
          {
            path: 'request', children: [
              { path: '', element: <ViewRequestsPage /> },
              { path: 'register', element: <NewRequestPage /> },
            ]
          },
        ]
      },
      { path: '*', element: <Navigate replace to={'/not-found'} /> }
    ]
  }
])