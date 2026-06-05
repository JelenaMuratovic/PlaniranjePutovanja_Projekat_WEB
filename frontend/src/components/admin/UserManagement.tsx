import { useEffect, useState, useRef } from "react";
import { authApi } from "../../api/auth/authApi";
import type { UserDto } from "../../models/auth/dtos";
import { Modal } from "../shared/Modal";
import { useAuth } from "../../hooks/useAuth";
import "../../styles/adminStyle.css";

export const UserManagement = () => {
  const { user: currentUser } = useAuth(); // Dobavljamo trenutno ulogovanog admina
  const [users, setUsers] = useState<UserDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [userPendingDelete, setUserPendingDelete] = useState<UserDto | null>(
    null,
  );
  const [isDeleting, setIsDeleting] = useState(false);

  const [notice, setNotice] = useState<string | null>(null);
  const [errorNotice, setErrorNotice] = useState<string | null>(null);

  const noticeTimer = useRef<number | null>(null);
  const errorTimer = useRef<number | null>(null);

  const loadUsers = async () => {
    setIsLoading(true);
    setErrorNotice(null);
    try {
      const data = await authApi.getAllUsers();
      setUsers(data);
    } catch (error) {
      console.error("Failed to fetch users", error);
      setErrorNotice("Could not load users. Please refresh the page.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    void loadUsers();

    return () => {
      if (noticeTimer.current) window.clearTimeout(noticeTimer.current);
      if (errorTimer.current) window.clearTimeout(errorTimer.current);
    };
  }, []);

  const showSuccess = (message: string) => {
    if (noticeTimer.current) window.clearTimeout(noticeTimer.current);
    setNotice(message);
    noticeTimer.current = window.setTimeout(() => setNotice(null), 4000);
  };

  const showError = (message: string) => {
    if (errorTimer.current) window.clearTimeout(errorTimer.current);
    setErrorNotice(message);
    errorTimer.current = window.setTimeout(() => setErrorNotice(null), 5000);
  };

  const confirmDeleteUser = async () => {
    if (!userPendingDelete) return;

    setIsDeleting(true);
    try {
      await authApi.deleteUser(userPendingDelete.id);
      showSuccess(
        `User ${userPendingDelete.firstName} ${userPendingDelete.lastName} deleted successfully.`,
      );

      setUsers((prevUsers) =>
        prevUsers.filter((u) => u.id !== userPendingDelete.id),
      );
      setUserPendingDelete(null);
    } catch (error) {
      console.error("Failed to delete user", error);
      showError("Failed to delete the user. Please try again.");
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <div className="admin-workspace">
      {/* Zaglavlje stranice */}
      <div className="admin-workspace__header">
        <h1 className="admin-workspace__title">User Management</h1>
        <p className="admin-workspace__subtitle">
          View, manage, and administrate registered user accounts.
        </p>
      </div>

      {/* Globalne poruke/obavestenja */}
      {notice && (
        <div className="admin-notice admin-notice--success">{notice}</div>
      )}
      {errorNotice && (
        <div className="admin-notice admin-notice--error">{errorNotice}</div>
      )}

      {/* Tabela sa podacima */}
      <div className="table-responsive">
        {isLoading ? (
          <div className="empty-state">
            <h5>Loading user directory</h5>
            <p>Gathering system account records...</p>
          </div>
        ) : users.length > 0 ? (
          <table className="admin-table">
            <thead>
              <tr>
                <th>First Name</th>
                <th>Last Name</th>
                <th>Email</th>
                <th>Role</th>
                <th style={{ textAlign: "center", width: "80px" }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {users.map((u) => {
                const isSelf = currentUser?.id === u.id;
                return (
                  <tr key={u.id}>
                    <td>
                      <strong>{u.firstName}</strong>
                    </td>
                    <td>{u.lastName}</td>
                    <td>{u.email}</td>
                    <td>
                      <span
                        className={`badge ${u.role === "Admin" ? "badge--admin" : "badge--user"}`}
                      >
                        {u.role}
                      </span>
                    </td>
                    <td style={{ textAlign: "center" }}>
                      <button
                        type="button"
                        className="button-delete-user"
                        title={
                          isSelf
                            ? "You cannot delete your own admin account"
                            : "Delete User Account"
                        }
                        disabled={isSelf}
                        onClick={() => setUserPendingDelete(u)}
                      >
                        🗑
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        ) : (
          <div className="empty-state empty-state--soft">
            <h5>No accounts found</h5>
            <p>There are no registered users logged in the system.</p>
          </div>
        )}
      </div>

      {/* MODAL ZA POTVRDU BRISANJA KORISNIKA */}
      {userPendingDelete && (
        <Modal
          title="Delete User Account"
          description={`Are you sure you want to permanently delete the account for ${userPendingDelete.firstName} ${userPendingDelete.lastName}?`}
          onClose={() => !isDeleting && setUserPendingDelete(null)}
        >
          <div className="stack">
            <p style={{ color: "var(--danger)", fontWeight: 500, margin: 0 }}>
              Warning: This will trigger a system-wide cascade. All associated
              travels, expenses, checklists, destinations, and activities
              belonging to this user will be deleted permanently.
            </p>
            <p style={{ margin: "0 0 12px 0" }}>This action is irreversible.</p>

            <div className="form-grid form-grid--two">
              <button
                type="button"
                className="button button--secondary"
                disabled={isDeleting}
                onClick={() => setUserPendingDelete(null)}
              >
                Cancel
              </button>
              <button
                type="button"
                className="button button--primary"
                style={{ background: "var(--danger)" }}
                disabled={isDeleting}
                onClick={confirmDeleteUser}
              >
                {isDeleting ? "Deleting..." : "Delete Account"}
              </button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
};

export default UserManagement;
