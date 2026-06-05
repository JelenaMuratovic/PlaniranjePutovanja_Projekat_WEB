import { useState } from "react";
import { Modal } from "../shared/Modal";
import { utilApi } from "../../api/util/utilApi";
import { getApiErrorMessage } from "../../helpers/apiError";
import type { ShareLinkResponseDto } from "../../models/util/dtos";

type ShareTravelModalProps = {
  travelId: string;
  travelName: string;
  onClose: () => void;
};

export const ShareTravelModal = ({
  travelId,
  travelName,
  onClose,
}: ShareTravelModalProps) => {
  const [accessLevel, setAccessLevel] = useState<number>(0);
  const [expirationDays, setExpirationDays] = useState<number>(7);
  const [shareData, setShareData] = useState<ShareLinkResponseDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleGenerateShare = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const result = await utilApi.generateShare(travelId, {
        accessLevel,
        expirationDays,
      });
      setShareData(result);
    } catch (err) {
      setError(getApiErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      title={`Share travel: ${travelName}`}
      description="Generate a secure link and QR code to share your travel plan."
      onClose={onClose}
    >
      <div className="stack">
        {error && <div className="error-box">{error}</div>}

        {!shareData ? (
          // Forma za odabir parametara deljenja
          <form onSubmit={handleGenerateShare} className="stack">
            <div className="form-field">
              <label htmlFor="accessLevel">Access Level</label>
              <select
                id="accessLevel"
                className="input"
                value={accessLevel}
                onChange={(e) => setAccessLevel(parseInt(e.target.value))}
              >
                <option value={0}>View (Read-only)</option>
                <option value={1}>Edit (Can modify data)</option>
              </select>
            </div>

            <div className="form-field">
              <label htmlFor="expirationDays">Link Expiration (Days)</label>
              <input
                id="expirationDays"
                type="number"
                className="input"
                min={1}
                max={30}
                value={expirationDays}
                onChange={(e) =>
                  setExpirationDays(parseInt(e.target.value) || 7)
                }
              />
            </div>

            <div className="form-grid form-grid--two" style={{ marginTop: 12 }}>
              <button
                type="button"
                className="button button--secondary"
                onClick={onClose}
              >
                Cancel
              </button>
              <button
                type="submit"
                className="button button--primary"
                disabled={loading}
              >
                {loading ? "Generating..." : "Generate Link & QR"}
              </button>
            </div>
          </form>
        ) : (
          // Prikaz generisanog QR koda i linka
          <div
            className="stack"
            style={{ alignItems: "center", textAlign: "center", gap: "16px" }}
          >
            <p>Scan the QR code below to access the plan:</p>

            {shareData.qrCodeImage?.fileContents && (
              <img
                src={`data:${shareData.qrCodeImage.contentType};base64,${shareData.qrCodeImage.fileContents}`}
                alt="Share QR Code"
                style={{
                  width: "200px",
                  height: "200px",
                  border: "1px solid #ccc",
                  borderRadius: "8px",
                  padding: "8px",
                }}
              />
            )}

            <div className="form-field" style={{ width: "100%" }}>
              <label>Target URL</label>
              <input
                type="text"
                className="input"
                readOnly
                value={shareData.targetUrl}
                onClick={(e) => (e.target as HTMLInputElement).select()}
              />
              <small style={{ color: "var(--color-text-muted)" }}>
                Click to select and copy link
              </small>
            </div>

            <button
              type="button"
              className="button button--primary"
              style={{ width: "100%" }}
              onClick={onClose}
            >
              Done
            </button>
          </div>
        )}
      </div>
    </Modal>
  );
};
