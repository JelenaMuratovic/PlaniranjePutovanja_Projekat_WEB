import { Link, Outlet } from "react-router-dom";

export const AuthLayout = () => {
  return (
    <main className="app-shell">
      <section className="auth-shell">
        <aside className="auth-hero">
          <div>
            <h1 className="auth-hero__title">Travel planning in one place.</h1>
            <p className="auth-hero__text">
              Organize your plans, destinations, activities, expenses, and
              checklists through a clean and focused interface.
            </p>
          </div>
        </aside>

        <section className="auth-panel">
          <div className="auth-panel__header">
            <h2 className="auth-panel__title">Welcome back</h2>
            <p className="auth-panel__subtitle">
              Sign in or create an account to continue.
            </p>
            <p className="auth-panel__subtitle" style={{ marginTop: "10px" }}>
              <Link to="/login">Login</Link> · <Link to="/register">Register</Link>
            </p>
          </div>

          <Outlet />
        </section>
      </section>
    </main>
  );
};
