import type { ReactNode } from "react";

type ModalProps = {
  title: string;
  description?: string;
  children: ReactNode;
  onClose: () => void;
};
// Univerzalni modal komponenta; u njega ubacimo bilo koji sadrzaj putem "children"
// modal = okvir; neka komponenta = sadrzaj koji ubacujemo u taj okvir
export const Modal = ({
  title,
  description,
  children,
  onClose,
}: ModalProps) => {
  return (
    <div className="modal-backdrop" role="presentation" onClick={onClose}>
      <div
        className="modal-card"
        role="dialog"
        aria-modal="true"
        aria-labelledby="modal-title"
        onClick={(event) => event.stopPropagation()}
      >
        <div className="modal-card__header">
          <div>
            <h2 id="modal-title">{title}</h2>
            {description ? <p>{description}</p> : null}
          </div>

          <button
            type="button"
            className="modal-card__close"
            onClick={onClose}
            aria-label="Close modal"
          >
            ×
          </button>
        </div>

        {children}
      </div>
    </div>
  );
};
