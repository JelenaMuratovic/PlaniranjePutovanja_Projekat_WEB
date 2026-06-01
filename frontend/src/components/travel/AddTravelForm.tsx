import type { FieldErrors, UseFormRegister } from "react-hook-form";
import type { CreateTravelFormValues } from "../../helpers/validation/travel";

type AddTravelFormProps = {
  register: UseFormRegister<CreateTravelFormValues>;
  errors: FieldErrors<CreateTravelFormValues>;
  isSubmitting: boolean;
  serverMessage: string | null;
  onSubmit: React.FormEventHandler<HTMLFormElement>;
  submitLabel?: string;
};

export const AddTravelForm = ({
  register,
  errors,
  isSubmitting,
  serverMessage,
  onSubmit,
  submitLabel = "Create travel plan",
}: AddTravelFormProps) => {
  return (
    <form className="stack" onSubmit={onSubmit} noValidate>
      {serverMessage ? <div className="message message--success">{serverMessage}</div> : null}

      <div className="field">
        <label htmlFor="name">Travel name</label>
        <input id="name" type="text" placeholder="Summer trip to Italy" {...register("name")} />
        {errors.name ? <span className="field__error">{errors.name.message}</span> : null}
      </div>

      <div className="field">
        <label htmlFor="description">Description</label>
        <textarea id="description" rows={4} placeholder="Short description of the trip" {...register("description")} />
        {errors.description ? <span className="field__error">{errors.description.message}</span> : null}
      </div>

      <div className="form-grid form-grid--two">
        <div className="field">
          <label htmlFor="startDate">Start date</label>
          <input id="startDate" type="date" {...register("startDate")} />
          {errors.startDate ? <span className="field__error">{errors.startDate.message}</span> : null}
        </div>

        <div className="field">
          <label htmlFor="endDate">End date</label>
          <input id="endDate" type="date" {...register("endDate")} />
          {errors.endDate ? <span className="field__error">{errors.endDate.message}</span> : null}
        </div>
      </div>

      <div className="form-grid form-grid--two">
        <div className="field">
          <label htmlFor="budget">Budget</label>
          <input id="budget" type="number" min="0" step="0.01" placeholder="1500" {...register("budget", { valueAsNumber: true })} />
          {errors.budget ? <span className="field__error">{errors.budget.message}</span> : null}
        </div>

        <div className="field">
          <label htmlFor="notes">Notes</label>
          <input id="notes" type="text" placeholder="Optional notes" {...register("notes")} />
          {errors.notes ? <span className="field__error">{errors.notes.message}</span> : null}
        </div>
      </div>

      <button className="button button--primary" type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Saving..." : submitLabel}
      </button>
    </form>
  );
};