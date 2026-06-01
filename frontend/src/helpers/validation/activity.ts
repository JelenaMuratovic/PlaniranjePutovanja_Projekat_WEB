import { z } from "zod";

export const activityStatusSchema = z.enum([
  "Planned",
  "Reserved",
  "Completed",
  "Cancelled",
]);

const timeSchema = z
  .string()
  .trim()
  .regex(/^([01]\d|2[0-3]):[0-5]\d$/, "Start time must be in HH:MM format.")
  .or(z.literal(""))
  .transform((value) => (value === "" ? null : value));

export const createActivitySchema = z.object({
  name: z
    .string()
    .trim()
    .min(1, "Activity name is required.")
    .min(2, "Activity name must be at least 2 characters long."),
  description: z
    .string()
    .trim()
    .min(1, "Description is required.")
    .min(5, "Description must be at least 5 characters long."),
  activityDate: z.string().min(1, "Activity date is required."),
  startTime: timeSchema,
  price: z.number().min(0, "Price cannot be negative."),
  status: activityStatusSchema,
});

export type CreateActivityFormValues = z.input<typeof createActivitySchema>;
