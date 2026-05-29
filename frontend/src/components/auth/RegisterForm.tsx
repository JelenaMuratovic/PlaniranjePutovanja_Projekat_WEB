import { Link } from "react-router-dom";
import type { FieldErrors, UseFormRegister } from "react-hook-form";
import type { RegisterFormValues } from "../../helpers/validation/auth";

type RegisterFormProps = {
  register: UseFormRegister<RegisterFormValues>;
  errors: FieldErrors<RegisterFormValues>;
  isSubmitting: boolean;
  serverMessage: string | null;
  onSubmit: React.FormEventHandler<HTMLFormElement>;
};

export const RegisterForm = ({
  register,
  errors,
  isSubmitting,
  serverMessage,
  onSubmit,
}: RegisterFormProps) => {
  return (
    <form className="stack" onSubmit={onSubmit} noValidate>
      {serverMessage ? (
        <div className="message message--error">{serverMessage}</div>
      ) : null}

      <div className="field">
        <label htmlFor="firstName">First name</label>
        <input
          id="firstName"
          type="text"
          placeholder="First name"
          {...register("firstName")}
        />
        {errors.firstName ? (
          <span className="field__error">{errors.firstName.message}</span>
        ) : null}
      </div>

      <div className="field">
        <label htmlFor="lastName">Last name</label>
        <input
          id="lastName"
          type="text"
          placeholder="Last name"
          {...register("lastName")}
        />
        {errors.lastName ? (
          <span className="field__error">{errors.lastName.message}</span>
        ) : null}
      </div>

      <div className="field">
        <label htmlFor="registerEmail">Email</label>
        <input
          id="registerEmail"
          type="email"
          placeholder="name@example.com"
          {...register("email")}
        />
        {errors.email ? (
          <span className="field__error">{errors.email.message}</span>
        ) : null}
      </div>

      <div className="field">
        <label htmlFor="registerPassword">Password</label>
        <input
          id="registerPassword"
          type="password"
          placeholder="Create a password"
          {...register("password")}
        />
        {errors.password ? (
          <span className="field__error">{errors.password.message}</span>
        ) : null}
      </div>

      <button
        className="button button--primary"
        type="submit"
        disabled={isSubmitting}
      >
        {isSubmitting ? "Creating..." : "Create account"}
      </button>

      <p className="auth-panel__subtitle">
        Already have an account? <Link to="/login">Sign in</Link>
      </p>
    </form>
  );
};