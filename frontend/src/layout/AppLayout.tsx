import { Link, Outlet } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

export const AppLayout = () => {
  const { user, logout } = useAuth();

  return (
    <main className="app-shell">
      <section className="app-card page-grid" style={{ padding: "24px" }}>
        <header className="dashboard-hero">
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              gap: "16px",
              alignItems: "center",
            }}
          >
            <div>
              <h1>Travel Planning</h1>
              <p>
                Logged in user: <strong>{user?.firstName} {user?.lastName}</strong>
              </p>
            </div>

            <button
              type="button"
              className="button button--secondary"
              onClick={logout}
            >
              Log out
            </button>
          </div>

          <nav style={{ display: "flex", gap: "12px", flexWrap: "wrap" }}>
            <Link className="button button--secondary" to="/dashboard">
              Dashboard
            </Link>
          </nav>
        </header>

        <Outlet />
      </section>
    </main>
  );
};
