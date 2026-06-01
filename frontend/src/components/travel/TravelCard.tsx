import { useEffect, useRef, useState } from "react";
import type { TravelDto } from "../../models/travel/travel/dtos";
import { formatDateOnly } from "../../helpers/date";

type TravelCardProps = {
  travel: TravelDto;
  onViewPlan: (travel: TravelDto) => void;
  onEdit: (travel: TravelDto) => void;
  onDelete: (travel: TravelDto) => void;
};

export const TravelCard = ({
  travel,
  onViewPlan,
  onEdit,
  onDelete,
}: TravelCardProps) => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement | null>(null);
  // Zatvaramo meni kada se klikne van njega
  useEffect(() => {
    const handleOutsideClick = (event: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setIsMenuOpen(false);
      }
    };

    document.addEventListener("mousedown", handleOutsideClick);

    return () => {
      document.removeEventListener("mousedown", handleOutsideClick);
    };
  }, []);

  return (
    <article className="travel-card">
      <div className="travel-card__badge">TRIP</div>

      <div className="travel-card__header">
        <div>
          <h3>{travel.name}</h3>
          <p>{travel.description}</p>
        </div>

        <div ref={menuRef} className="travel-card__menu-wrap">
          <button
            type="button"
            className="travel-card__menu"
            aria-label={`Actions for ${travel.name}`}
            aria-expanded={isMenuOpen}
            aria-haspopup="menu"
            onClick={() => setIsMenuOpen((current) => !current)}
          >
            ...
          </button>

          {isMenuOpen ? (
            <div role="menu" className="travel-card__menu-panel">
              <button
                type="button"
                role="menuitem"
                className="travel-card__menu-item"
                onClick={() => {
                  setIsMenuOpen(false);
                  onEdit(travel);
                }}
              >
                <span aria-hidden="true">✎</span>
                <span>Edit</span>
              </button>

              <button
                type="button"
                role="menuitem"
                className="travel-card__menu-item travel-card__menu-item--danger"
                onClick={() => {
                  setIsMenuOpen(false);
                  onDelete(travel);
                }}
              >
                <span aria-hidden="true">🗑</span>
                <span>Delete</span>
              </button>
            </div>
          ) : null}
        </div>
      </div>

      <div className="travel-card__meta">
        <span>{formatDateOnly(travel.startDate)}</span>
        <span>to</span>
        <span>{formatDateOnly(travel.endDate)}</span>
      </div>

      <div className="travel-card__footer">
        <div>
          <span className="travel-card__footer-label">Destinations</span>
          <strong>{travel.destinationCount}</strong>
        </div>

        <button
          type="button"
          className="button button--primary travel-card__cta"
          onClick={() => onViewPlan(travel)}
        >
          View Plan
        </button>
      </div>
    </article>
  );
};
