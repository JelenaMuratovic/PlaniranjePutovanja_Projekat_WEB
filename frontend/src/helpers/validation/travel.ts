import { z } from "zod";

export const createTravelSchema = z
  .object({
    name: z.string().trim().min(1, "Travel name is required.").min(3, "Travel name must be at least 3 characters long."),
    description: z.string().trim().min(1, "Description is required.").min(5, "Description must be at least 5 characters long."),
    startDate: z.string().min(1, "Start date is required."),
    endDate: z.string().min(1, "End date is required."),
    budget: z.number().min(0, "Budget cannot be negative."),
    notes: z.string().trim(),
  })
  .refine(
    (values) => new Date(values.endDate) >= new Date(values.startDate),
    {
      message: "End date cannot be before start date.",
      path: ["endDate"],
    },
  );

export type CreateTravelFormValues = z.input<typeof createTravelSchema>;