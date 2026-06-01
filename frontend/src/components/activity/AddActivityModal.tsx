import { useEffect, useMemo, useState, type FormEvent } from "react";
import { Modal } from "../shared/Modal";
import type { DestinationDto } from "../../models/travel/destination/dtos";
import type {
  ActivityStatus,
  ActivityDto,
  CreateActivityDto,
} from "../../models/travel/activity/dtos";
import {
  addDaysToDateOnly,
  formatDateOnly,
  formatDateOnlyForApi,
  parseDateOnly,
} from "../../helpers/date";

export type ActivityDay = {
  dayNumber: number;
  activityDate: string;
};

type AddActivityModalProps = {
  open: boolean;
  destination: DestinationDto;
  travelStartDate: string;
  onClose: () => void;
  onSave: (values: CreateActivityDto) => Promise<void> | void;
  title?: string;
  modalDescription?: string;
  submitLabel?: string;
  initialValues?: ActivityDto | null;
};

const activityStatusOptions: Array<{ label: ActivityStatus; value: number }> = [
  { label: "Planned", value: 0 },
  { label: "Reserved", value: 1 },
  { label: "Completed", value: 2 },
  { label: "Cancelled", value: 3 },
];

const buildActivityDate = (
  travelStartDate: string,
  destinationDayOffset: number,
): string => {
  const date = addDaysToDateOnly(travelStartDate, destinationDayOffset);
  return `${formatDateOnlyForApi(date)}T00:00:00`;
};

const normalizeTimeForInput = (time: string | null | undefined): string => {
  if (!time) {
    return "";
  }

  return time.length >= 5 ? time.slice(0, 5) : time;
};

const formatTimeForApi = (time: string): string | null => {
  const trimmedTime = time.trim();

  if (!trimmedTime) {
    return null;
  }

  return trimmedTime.length === 5 ? `${trimmedTime}:00` : trimmedTime;
};

const formatDateForDisplay = (value: string): string => {
  return formatDateOnly(value, {
    weekday: "short",
    day: "2-digit",
    month: "short",
    year: "numeric",
  });
};

export const AddActivityModal = ({
  open,
  destination,
  travelStartDate,
  onClose,
  onSave,
  title = `Add activity for ${destination.name}`,
  modalDescription = "Choose a day for this destination, then enter the activity details.",
  submitLabel = "Add activity",
  initialValues = null,
}: AddActivityModalProps) => {
  const days = useMemo(
    () =>
      Array.from({ length: destination.daysSpent }, (_, index) => index + 1),
    [destination.daysSpent],
  );

  const [selectedDay, setSelectedDay] = useState(1);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [startTime, setStartTime] = useState("");
  const [price, setPrice] = useState(0);
  const [status, setStatus] = useState<number>(0);

  const parseSelectedDay = (activityDate: string): number => {
    const activity = parseDateOnly(activityDate);
    const travel = parseDateOnly(travelStartDate);
    if (Number.isNaN(activity.getTime()) || Number.isNaN(travel.getTime())) {
      return 1;
    }

    const millisPerDay = 1000 * 60 * 60 * 24;
    const diff = Math.max(
      0,
      Math.floor((activity.getTime() - travel.getTime()) / millisPerDay),
    );

    return diff + 1;
  };

  const defaultInitialValues = initialValues ?? null;

  useEffect(() => {
    if (!open) {
      return;
    }

    setSelectedDay(
      defaultInitialValues ? parseSelectedDay(defaultInitialValues.activityDate) : 1,
    );
    setName(defaultInitialValues?.name ?? "");
    setDescription(defaultInitialValues?.description ?? "");
    setStartTime(normalizeTimeForInput(defaultInitialValues?.startTime));
    setPrice(defaultInitialValues?.price ?? 0);
    setStatus(
      defaultInitialValues
        ? activityStatusOptions.find(
            (option) => option.label === defaultInitialValues.status,
          )?.value ?? 0
        : 0,
    );
  }, [open, destination.id, defaultInitialValues, travelStartDate]);

  if (!open) return null;

  const selectedDate = buildActivityDate(travelStartDate, selectedDay - 1);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    await onSave({
      name: name.trim(),
      description: description.trim(),
      activityDate: selectedDate,
      startTime: formatTimeForApi(startTime),
      price,
      status,
    } as unknown as CreateActivityDto);
  };

  return (
    <Modal
      title={title}
      description={modalDescription}
      onClose={onClose}
    >
      <div className="activity-modal">
        <div className="activity-modal__days" aria-label="Destination days">
          {days.map((dayNumber) => (
            <button
              key={dayNumber}
              type="button"
              className={`activity-day-pill ${selectedDay === dayNumber ? "activity-day-pill--active" : ""}`}
              onClick={() => setSelectedDay(dayNumber)}
            >
              Day {dayNumber}
            </button>
          ))}
        </div>

        <div className="activity-modal__date-card" aria-live="polite">
          <span className="activity-modal__date-label">Selected date</span>
          <strong className="activity-modal__date-value">
            {formatDateForDisplay(selectedDate)}
          </strong>
        </div>

        <form onSubmit={handleSubmit} className="stack">
          <div className="form-grid form-grid--two">
            <div className="field">
              <label htmlFor="activity-name">Name</label>
              <input
                id="activity-name"
                value={name}
                onChange={(event) => setName(event.target.value)}
                required
              />
            </div>

            <div className="field">
              <label htmlFor="activity-status">Status</label>
              <select
                id="activity-status"
                value={status}
                onChange={(event) => setStatus(Number(event.target.value))}
              >
                {activityStatusOptions.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="field">
            <label htmlFor="activity-description">Description</label>
            <textarea
              id="activity-description"
              rows={3}
              value={description}
              onChange={(event) => setDescription(event.target.value)}
              required
            />
          </div>

          <div className="form-grid form-grid--two">
            <div className="field">
              <label htmlFor="activity-start-time">Start time</label>
              <input
                id="activity-start-time"
                type="time"
                value={startTime}
                onChange={(event) => setStartTime(event.target.value)}
              />
            </div>

            <div className="field">
              <label htmlFor="activity-price">Price</label>
              <input
                id="activity-price"
                type="number"
                min={0}
                step="0.01"
                value={price}
                onChange={(event) => setPrice(Number(event.target.value))}
              />
            </div>
          </div>

          <div className="form-grid form-grid--two">
            <button
              type="button"
              className="button button--secondary"
              onClick={onClose}
            >
              Cancel
            </button>
            <button type="submit" className="button button--primary">
              {submitLabel}
            </button>
          </div>
        </form>
      </div>
    </Modal>
  );
};

export default AddActivityModal;
