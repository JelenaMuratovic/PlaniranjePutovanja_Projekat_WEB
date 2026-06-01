import { useState, type FormEvent } from "react";
import { Modal } from "../shared/Modal";
import type {
  CreateDestinationDto,
  DestinationDto,
} from "../../models/travel/destination/dtos";

type AddDestinationModalProps = {
  open: boolean;
  onClose: () => void;
  onSave: (vals: CreateDestinationDto) => Promise<void> | void;
  title?: string;
  description?: string;
  submitLabel?: string;
  initialValues?: DestinationDto | null;
};

const initialState = {
  name: "",
  country: "",
  city: "",
  latitude: "",
  longitude: "",
  daysSpent: 1,
  description: "",
  notes: "",
};

export const AddDestinationModal = ({
  open,
  onClose,
  onSave,
  title = "Add destination",
  description = "Add a stop for this trip.",
  submitLabel = "Add destination",
  initialValues = null,
}: AddDestinationModalProps) => {
  const [name, setName] = useState(initialValues?.name ?? initialState.name);
  const [country, setCountry] = useState(
    initialValues?.country ?? initialState.country,
  );
  const [city, setCity] = useState(initialValues?.city ?? initialState.city);
  const [latitude, setLatitude] = useState(
    initialValues?.latitude?.toString() ?? initialState.latitude,
  );
  const [longitude, setLongitude] = useState(
    initialValues?.longitude?.toString() ?? initialState.longitude,
  );
  const [daysSpent, setDaysSpent] = useState(
    initialValues?.daysSpent ?? initialState.daysSpent,
  );
  const [descriptionValue, setDescription] = useState(
    initialValues?.description ?? initialState.description,
  );
  const [notes, setNotes] = useState(initialValues?.notes ?? initialState.notes);

  if (!open) return null;

  const resetForm = () => {
    setName(initialValues?.name ?? initialState.name);
    setCountry(initialValues?.country ?? initialState.country);
    setCity(initialValues?.city ?? initialState.city);
    setLatitude(initialValues?.latitude?.toString() ?? initialState.latitude);
    setLongitude(
      initialValues?.longitude?.toString() ?? initialState.longitude,
    );
    setDaysSpent(initialValues?.daysSpent ?? initialState.daysSpent);
    setDescription(initialValues?.description ?? initialState.description);
    setNotes(initialValues?.notes ?? initialState.notes);
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    await onSave({
      name: name.trim(),
      country: country.trim(),
      city: city.trim(),
      latitude: latitude.trim() ? Number(latitude) : null,
      longitude: longitude.trim() ? Number(longitude) : null,
      daysSpent: Math.max(1, Math.floor(daysSpent)),
      description: descriptionValue.trim(),
      notes: notes.trim(),
    });

    resetForm();
    onClose();
  };

  return (
    <Modal
      title={title}
      description={description}
      onClose={onClose}
    >
      <form onSubmit={handleSubmit} className="stack">
        <div className="form-grid form-grid--two">
          <div className="field">
            <label htmlFor="destination-name">Name</label>
            <input
              id="destination-name"
              value={name}
              onChange={(event) => setName(event.target.value)}
              required
            />
          </div>

          <div className="field">
            <label htmlFor="destination-country">Country</label>
            <input
              id="destination-country"
              value={country}
              onChange={(event) => setCountry(event.target.value)}
              required
            />
          </div>
        </div>

        <div className="form-grid form-grid--two">
          <div className="field">
            <label htmlFor="destination-city">City</label>
            <input
              id="destination-city"
              value={city}
              onChange={(event) => setCity(event.target.value)}
              required
            />
          </div>

          <div className="field">
            <label htmlFor="destination-days-spent">Days spent</label>
            <input
              id="destination-days-spent"
              type="number"
              min={1}
              value={daysSpent}
              onChange={(event) => setDaysSpent(Number(event.target.value))}
              required
            />
          </div>
        </div>

        <div className="form-grid form-grid--two">
          <div className="field">
            <label htmlFor="destination-latitude">Latitude</label>
            <input
              id="destination-latitude"
              inputMode="decimal"
              value={latitude}
              onChange={(event) => setLatitude(event.target.value)}
              placeholder="Optional"
            />
          </div>

          <div className="field">
            <label htmlFor="destination-longitude">Longitude</label>
            <input
              id="destination-longitude"
              inputMode="decimal"
              value={longitude}
              onChange={(event) => setLongitude(event.target.value)}
              placeholder="Optional"
            />
          </div>
        </div>

        <div className="field">
          <label htmlFor="destination-description">Description</label>
          <textarea
            id="destination-description"
            rows={3}
            value={descriptionValue}
            onChange={(event) => setDescription(event.target.value)}
            required
          />
        </div>

        <div className="field">
          <label htmlFor="destination-notes">Notes</label>
          <textarea
            id="destination-notes"
            rows={3}
            value={notes}
            onChange={(event) => setNotes(event.target.value)}
            placeholder="Optional notes for this stop"
          />
        </div>

        <div className="form-grid form-grid--two">
          <button
            type="button"
            className="button button--secondary"
            onClick={() => {
              resetForm();
              onClose();
            }}
          >
            Cancel
          </button>
          <button type="submit" className="button button--primary">
            {submitLabel}
          </button>
        </div>
      </form>
    </Modal>
  );
};

export default AddDestinationModal;
