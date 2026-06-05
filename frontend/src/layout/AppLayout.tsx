import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

export const AppLayout = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const isAdmin = user?.role === "Admin";

  const handleLogoutClick = () => {
    // Cistimo sve session storage parametre koji prate deljeni link
    sessionStorage.removeItem("active_share_token");
    sessionStorage.removeItem("redirectAfterLogin");

    // Pozivamo standardni logout koji cisti localStorage/state
    logout();

    // Eksplicitno preusmeravamo na login
    navigate("/login", { replace: true });
  };

  return (
    <main className="app-layout">
      <aside className="sidebar">
        <div className="sidebar__brand">
          <div className="sidebar__logo">TP</div>
          <div>
            <div className="sidebar__title">Travel Planning</div>
            <div className="sidebar__subtitle">Travel planner workspace</div>
          </div>
        </div>
        <div className="sidebar__intro">
          <p className="sidebar__intro-eyebrow">Welcome</p>
          <h2 className="sidebar__intro-title">
            Hi, {user ? user.firstName : "Guest"}
          </h2>
          <p className="sidebar__intro-text">Ready to plan your next trip?</p>
        </div>

        <nav className="sidebar__nav" aria-label="Primary navigation">
          <NavLink
            className={({ isActive }) =>
              `sidebar__item ${isActive ? "sidebar__item--active" : ""}`
            }
            to="/dashboard"
          >
            <span className="sidebar__icon">▣</span>
            <span>Dashboard</span>
          </NavLink>

          {isAdmin && (
            <NavLink
              className={({ isActive }) =>
                `sidebar__item ${isActive ? "sidebar__item--active" : ""}`
              }
              to="/admin/users"
            >
              <span className="sidebar__icon">◫</span>
              <span>User Management</span>
            </NavLink>
          )}
        </nav>

        <div className="sidebar__footer">
          <button
            type="button"
            className="sidebar__item sidebar__item--ghost"
            onClick={handleLogoutClick}
          >
            <span className="sidebar__icon">⇦</span>
            <span>Log out</span>
          </button>
        </div>
      </aside>

      <section className="app-shell app-shell--workspace">
        <Outlet />
      </section>
    </main>
  );
};
