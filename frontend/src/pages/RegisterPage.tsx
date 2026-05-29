import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useNavigate } from "react-router-dom";
import { authApi } from "../api/auth/authApi";
import { RegisterForm } from "../components/auth/RegisterForm";
import { getApiErrorMessage } from "../helpers/apiError";
import { useAuth } from "../hooks/useAuth";
import {
  registerSchema,
  type RegisterFormValues,
} from "../helpers/validation/auth";

export const RegisterPage = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [serverMessage, setServerMessage] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
    },
  });

  const onSubmit = async (values: RegisterFormValues) => {
    try {
      setServerMessage(null);
      const response = await authApi.register(values);

      if (!response.isSuccess) {
        setServerMessage(response.message || "Registration failed.");
        return;
      }

      if (response.accessToken && response.user) {
        login(response);
        navigate("/dashboard", { replace: true });
        return;
      }

      setServerMessage(response.message || "Account created. Please sign in.");
      navigate("/login", { replace: true });
    } catch (error) {
      setServerMessage(getApiErrorMessage(error));
    }
  };

  return (
    <RegisterForm
      register={register}
      errors={errors}
      isSubmitting={isSubmitting}
      serverMessage={serverMessage}
      onSubmit={handleSubmit(onSubmit)}
    />
  );
};
