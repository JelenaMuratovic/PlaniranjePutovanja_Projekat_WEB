import { useEffect, useRef, useState } from "react";
import type { DestinationDto } from "../../models/travel/destination/dtos";
import { formatDateOnly } from "../../helpers/date";

type DestinationSchedule = {
  startDay: number;
  endDay: number;
  startDate: string;
  endDate: string;
};

type DestinationCardProps = {
  destination: DestinationDto;
  schedule?: DestinationSchedule | null;
  onAddActivity?: (destination: DestinationDto) => void;
  onEdit?: (destination: DestinationDto) => void;
  onDelete?: (destination: DestinationDto) => void;
};

export const DestinationCard = ({
  destination,
  schedule,
  onAddActivity,
  onEdit,
  onDelete,
}: DestinationCardProps) => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement | null>(null);

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
    <article className="destination-card">
      <div className="destination-card__header">
        <div>
          <p className="destination-card__eyebrow">
            {destination.city}, {destination.country}
          </p>
          <h4 className="destination-card__title">{destination.name}</h4>
        </div>

        <div ref={menuRef} className="travel-card__menu-wrap">
          <button
            type="button"
            className="travel-card__menu"
            aria-label={`Actions for ${destination.name}`}
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
                  onEdit?.(destination);
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
                  onDelete?.(destination);
                }}
              >
                <span aria-hidden="true">🗑</span>
                <span>Delete</span>
              </button>
            </div>
          ) : null}
        </div>
      </div>

      {schedule ? (
        <p className="destination-card__range">
          {formatDateOnly(schedule.startDate)} - {formatDateOnly(schedule.endDate)}
        </p>
      ) : null}

      <p className="destination-card__description">
        {destination.description || "No description provided yet."}
      </p>

      {destination.notes ? (
        <div className="destination-card__notes">{destination.notes}</div>
      ) : null}

      <div className="destination-card__footer">
        <div className="destination-card__meta">
          <span>Activities</span>
          <strong>{destination.activityCount}</strong>
        </div>

        <button
          type="button"
          className="button button--secondary button--sm destination-card__action"
          onClick={() => onAddActivity?.(destination)}
        >
          + Add activity
        </button>
      </div>
    </article>
  );
};

export default DestinationCard;
