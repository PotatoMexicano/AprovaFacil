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
import ViewMyRequestsPage from "../features/requests/view-my-requests/Page";
import ViewPendingRequestsPage from "../features/requests/view-pending-requests/Page";
import AdminRoute from "../components/admin-route";
import ViewRequest from "../features/requests/view-request/Page";
import ViewAllRequestsPage from "../features/requests/view-all-requests/Page";
import FinanceRoute from "../components/finance-route";
import ViewApprovedRequestsPage from "../features/requests/view-approved-requests/Page";
import ViewFinishedRequestsPage from "../features/requests/view-finished-requests/Page";

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
              { path: '', element: <ViewCompaniesPage /> },
              { path: 'register', element: <NewCompanyPage /> },
              { path: 'edit/:id', element: <EditCompanyPage /> },
              {
                element: <AdminRoute />,
                children: []
              },
            ]
          },
          {
            path: 'request', children: [
              { path: '', element: <ViewMyRequestsPage /> },
              { path: 'register', element: <NewRequestPage /> },
              { path: ':id', element: <ViewRequest /> },
              {
                element: <AdminRoute />,
                children: [
                  { path: 'pending', element: <ViewPendingRequestsPage /> },
                  { path: 'all', element: <ViewAllRequestsPage /> },
                ]
              },
              {
                element: <FinanceRoute />,
                children: [
                  { path: 'finished', element: <ViewFinishedRequestsPage /> },
                  { path: 'approved', element: <ViewApprovedRequestsPage /> },
                  { path: 'all', element: <ViewAllRequestsPage /> },
                ]
              }
            ]
          },
          {
            path: 'users', children: [
              { path: '', element: <ViewUsersPage /> },
              {
                element: <AdminRoute />,
                children: [
                  { path: "register", element: <NewUserPage /> },

                ]
              },
              { path: 'edit/:id', element: <EditUserPage /> },
            ]
          }
        ]
      },
      { path: '*', element: <Navigate replace to={'/not-found'} /> }
    ]
  }
])