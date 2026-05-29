import { z } from "zod";

export const loginSchema = z.object({
  email: z
    .string()
    .trim()
    .min(1, "Email is required.")
    .email("Enter a valid email address."),
  password: z
    .string()
    .trim()
    .min(1, "Password is required.")
    .min(6, "Password must be at least 6 characters long."),
});

export const registerSchema = z
  .object({
    firstName: z
      .string()
      .trim()
      .min(1, "First name is required.")
      .min(2, "First name must be at least 2 characters long."),
    lastName: z
      .string()
      .trim()
      .min(1, "Last name is required.")
      .min(2, "Last name must be at least 2 characters long."),
    email: z
      .string()
      .trim()
      .min(1, "Email is required.")
      .email("Enter a valid email address."),
    password: z
      .string()
      .trim()
      .min(1, "Password is required.")
      .min(6, "Password must be at least 6 characters long."),
  })
  .superRefine((values, context) => {
    if (values.firstName === values.lastName) {
      context.addIssue({
        code: "custom",
        path: ["lastName"],
        message: "First name and last name should not be the same.",
      });
    }
  });

export type LoginFormValues = z.infer<typeof loginSchema>;
export type RegisterFormValues = z.infer<typeof registerSchema>;
