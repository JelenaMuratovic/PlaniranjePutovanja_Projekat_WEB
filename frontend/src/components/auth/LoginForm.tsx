import { Link } from "react-router-dom";
import type { FieldErrors, UseFormRegister } from "react-hook-form";
import type { LoginFormValues } from "../../helpers/validation/auth";

type LoginFormProps = {
  register: UseFormRegister<LoginFormValues>;
  errors: FieldErrors<LoginFormValues>;
  isSubmitting: boolean;
  serverMessage: string | null;
  onSubmit: React.FormEventHandler<HTMLFormElement>;
};

export const LoginForm = ({
  register,
  errors,
  isSubmitting,
  serverMessage,
  onSubmit,
}: LoginFormProps) => {
  return (
    <form className="stack" onSubmit={onSubmit} noValidate>
      {serverMessage ? (
        <div className="message message--error">{serverMessage}</div>
      ) : null}

      <div className="field">
        <label htmlFor="email">Email</label>
        <input
          id="email"
          type="email"
          placeholder="name@example.com"
          {...register("email")}
        />
        {errors.email ? (
          <span className="field__error">{errors.email.message}</span>
        ) : null}
      </div>

      <div className="field">
        <label htmlFor="password">Password</label>
        <input
          id="password"
          type="password"
          placeholder="Enter your password"
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
        {isSubmitting ? "Signing in..." : "Sign in"}
      </button>

      <p className="auth-panel__subtitle">
        Don't have an account? <Link to="/register">Register</Link>
      </p>
    </form>
  );
};