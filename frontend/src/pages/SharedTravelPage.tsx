import { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { utilApi } from "../api/util/utilApi";
import { TravelWorkspacePanel } from "../components/dashboard/TravelWorkspacePanel";
import { AUTH_TOKEN_KEY } from "../helpers/authStorage";

// Pomocna funkcija za dekodiranje JWT-a na frontu (da procitamo AccessLevel bez backenda)
const parseJwt = (token: string) => {
  try {
    return JSON.parse(atob(token.split(".")[1]));
  } catch (e) {
    return null;
  }
};

export const SharedTravelPage = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const token = searchParams.get("token"); // Izvlacenje tokena iz URL-a

  const [travel, setTravel] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isReadOnly, setIsReadOnly] = useState(true);

  useEffect(() => {
    if (!token) {
      setError("Nedostaje autorizacioni token za pristup putovanju.");
      setLoading(false);
      return;
    }

    sessionStorage.setItem("active_share_token", token);

    const isUserLoggedIn = !!localStorage.getItem(AUTH_TOKEN_KEY);
    // Dekodiramo token sa linka da vidimo nivo pristupa
    const decodedToken = parseJwt(token);
    const accessLevel = decodedToken?.AccessLevel; // "View" ili "Edit"
    const isEditToken = accessLevel === "Edit" || accessLevel === 1;
    const isViewToken = accessLevel === "View" || accessLevel === 0;

    if (isEditToken && !isUserLoggedIn) {
      const currentUrl = window.location.pathname + window.location.search;
      sessionStorage.setItem("redirectAfterLogin", currentUrl);
      alert(
        "This shared link requires permission to edit. Please login to your account first.",
      );
      navigate("/login", { replace: true });
      return;
    }
    setIsReadOnly(isViewToken);

    // Postavljamo da li je front u ReadOnly modu
    setIsReadOnly(accessLevel === "View");

    utilApi
      .getSharedTravel(token)
      .then((data) => {
        setTravel(data);
        setLoading(false);
      })
      .catch((err) => {
        setError(
          "The sharing link has expired or you do not have permission to access it.",
        );
        setLoading(false);
      });
  }, [token, navigate]);

  if (loading) return <div className="loading-box">Loading travel plan...</div>;
  if (error)
    return (
      <div className="error-box" style={{ color: "red", padding: "20px" }}>
        {error}
      </div>
    );

  return (
    <div className="shared-page-container">
      <div
        style={{
          backgroundColor: isReadOnly ? "#e3f2fd" : "#e8f5e9",
          padding: "12px",
          textAlign: "center",
          fontWeight: "bold",
        }}
      >
        {isReadOnly
          ? "You are viewing a shared travel plan (Read-only mode)"
          : "You are editing a shared travel plan (As a logged-in collaborator)"}
      </div>

      {travel && (
        <TravelWorkspacePanel travel={travel} isReadOnly={isReadOnly} />
      )}
    </div>
  );
};
