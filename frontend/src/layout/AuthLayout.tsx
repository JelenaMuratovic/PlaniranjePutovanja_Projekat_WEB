import { Link, Outlet, useLocation } from "react-router-dom";

export const AuthLayout = () => {
  const { pathname } = useLocation();
  const isAuthCentered = pathname === "/login" || pathname === "/register";

  return (
    <main className="app-shell">
      <section
        className={`auth-shell ${isAuthCentered ? "auth-shell--centered" : ""}`}
      >
        <section className="auth-panel">
          <div className="auth-panel__header">
            <p className="auth-panel__eyebrow">Workspace access</p>
            <h2 className="auth-panel__title">Welcome back</h2>
            <p className="auth-panel__subtitle">
              Sign in or create an account to continue.
            </p>

            <div className="auth-panel__links">
              <Link to="/login">Login</Link>
              <span>·</span>
              <Link to="/register">Register</Link>
            </div>
          </div>

          <Outlet />
        </section>
      </section>
    </main>
  );
};
