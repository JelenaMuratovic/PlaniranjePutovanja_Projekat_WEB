import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useLocation, useNavigate } from "react-router-dom";
import { authApi } from "../api/auth/authApi";
import { LoginForm } from "../components/auth/LoginForm";
import { getApiErrorMessage } from "../helpers/apiError";
import { useAuth } from "../hooks/useAuth";
import { loginSchema, type LoginFormValues } from "../helpers/validation/auth";

export const LoginPage = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [serverMessage, setServerMessage] = useState<string | null>(null);

  const from =
    (location.state as { from?: { pathname?: string } } | null)?.from
      ?.pathname ?? "/dashboard";

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const onSubmit = async (values: LoginFormValues) => {
    try {
      setServerMessage(null);
      const response = await authApi.login(values);

      if (!response.isSuccess || !response.accessToken || !response.user) {
        setServerMessage(response.message || "Login failed.");
        return;
      }

      login(response);
      // dodala
      const redirectTo = sessionStorage.getItem("redirectAfterLogin");
      if (redirectTo) {
        sessionStorage.removeItem("redirectAfterLogin");
        // Preusmeravamo ga direktno na sacuvani URL deljenog putovanja
        navigate(redirectTo, { replace: true });
      } else {
        // Ako nije dosao preko QR koda, ide na dashboard ili odatle odakle je dosao
        navigate(from, { replace: true });
      }
    } catch (error) {
      setServerMessage(getApiErrorMessage(error));
    }
  };

  return (
    <LoginForm
      register={register}
      errors={errors}
      isSubmitting={isSubmitting}
      serverMessage={serverMessage}
      onSubmit={handleSubmit(onSubmit)}
    />
  );
};
