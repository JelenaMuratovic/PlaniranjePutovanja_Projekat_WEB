import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

type ProtectedRouteProps = {
  allowedRoles?: string[];
};

export const ProtectedRoute = ({ allowedRoles }: ProtectedRouteProps) => {
  const { isAuthenticated, loading, user } = useAuth();
  const location = useLocation();

  if (loading) {
    return <div className="app-shell app-shell--centered">Loading...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  if (
    allowedRoles &&
    allowedRoles.length > 0 &&
    (!user || !allowedRoles.includes(user.role))
  ) {
    return (
      <div className="app-shell app-shell--centered">
        <div className="app-card app-card--loading" role="alert">
          <h2>Access denied</h2>
          <p>
            Your role does not have permission to access this page.
          </p>
        </div>
      </div>
    );
  }

  return <Outlet />;
};
