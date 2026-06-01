import { useEffect, useRef, useState } from "react";
import type { ActivityDto } from "../../models/travel/activity/dtos";
import type { DestinationDto } from "../../models/travel/destination/dtos";

type DestinationSchedule = DestinationDto & {
  startDay: number;
  endDay: number;
  startDate: string;
  endDate: string;
};

type ActivityCardProps = {
  activity: ActivityDto;
  destination: DestinationSchedule;
  onEdit?: (activity: ActivityDto, destination: DestinationSchedule) => void;
  onDelete?: (activity: ActivityDto, destination: DestinationSchedule) => void;
};

const formatActivityDate = (value: string): string => {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return value;
  }

  return date.toLocaleDateString("en-GB", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  });
};

export const ActivityCard = ({
  activity,
  destination,
  onEdit,
  onDelete,
}: ActivityCardProps) => {
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
    <article className="activity-card">
      <div className="activity-card__header">
        <div>
          <p className="activity-card__eyebrow">
            {destination.city}, {destination.country}
          </p>
          <h5 className="activity-card__title">{activity.name}</h5>
        </div>

        <div ref={menuRef} className="travel-card__menu-wrap">
          <button
            type="button"
            className="travel-card__menu"
            aria-label={`Actions for ${activity.name}`}
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
                  onEdit?.(activity, destination);
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
                  onDelete?.(activity, destination);
                }}
              >
                <span aria-hidden="true">🗑</span>
                <span>Delete</span>
              </button>
            </div>
          ) : null}
        </div>
      </div>

      <div className="activity-card__status">{activity.status}</div>

      <p className="activity-card__description">{activity.description}</p>

      <div className="activity-card__meta">
        <span>{formatActivityDate(activity.activityDate)}</span>
        <span>{activity.startTime ?? "No time"}</span>
        <span>
          {activity.price.toLocaleString("en-GB", {
            style: "currency",
            currency: "EUR",
          })}
        </span>
      </div>
    </article>
  );
};

export default ActivityCard;
