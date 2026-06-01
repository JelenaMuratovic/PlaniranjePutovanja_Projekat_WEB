import { z } from "zod";

const nullableNumber = z.preprocess((value) => {
  if (value === "" || value === null || value === undefined) {
    return null;
  }

  if (typeof value === "number" && Number.isNaN(value)) {
    return null;
  }

  return value;
}, z.number().nullable());

export const createDestinationSchema = z.object({
  name: z
    .string()
    .trim()
    .min(1, "Destination name is required.")
    .min(2, "Destination name must be at least 2 characters long."),
  country: z
    .string()
    .trim()
    .min(1, "Country is required.")
    .min(2, "Country must be at least 2 characters long."),
  city: z
    .string()
    .trim()
    .min(1, "City is required.")
    .min(2, "City must be at least 2 characters long."),
  latitude: nullableNumber.refine(
    (value) => value === null || (value >= -90 && value <= 90),
    {
      message: "Latitude must be between -90 and 90.",
    },
  ),
  longitude: nullableNumber.refine(
    (value) => value === null || (value >= -180 && value <= 180),
    {
      message: "Longitude must be between -180 and 180.",
    },
  ),
  daysSpent: z
    .number()
    .int("Days spent must be a whole number.")
    .min(1, "Days spent must be at least 1 day."),
  description: z
    .string()
    .trim()
    .min(1, "Description is required.")
    .min(5, "Description must be at least 5 characters long."),
  notes: z.string().trim(),
});

export type CreateDestinationFormValues = z.input<
  typeof createDestinationSchema
>;
