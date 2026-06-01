const padDatePart = (value: number): string => value.toString().padStart(2, "0");

export const getDateOnlyPart = (value: string): string => value.split("T")[0] ?? value;

export const parseDateOnly = (value: string): Date => {
  const [yearPart, monthPart, dayPart] = getDateOnlyPart(value).split("-");
  const year = Number(yearPart);
  const month = Number(monthPart);
  const day = Number(dayPart);

  if (!year || !month || !day) {
    return new Date(value);
  }

  return new Date(year, month - 1, day);
};

export const formatDateOnly = (
  value: string | Date,
  options?: Intl.DateTimeFormatOptions,
): string => {
  const date = value instanceof Date ? value : parseDateOnly(value);

  if (Number.isNaN(date.getTime())) {
    return typeof value === "string" ? value : "";
  }

  return date.toLocaleDateString("en-GB", options);
};

export const formatDateOnlyForApi = (value: Date): string => {
  if (Number.isNaN(value.getTime())) {
    return "";
  }

  const year = value.getFullYear();
  const month = padDatePart(value.getMonth() + 1);
  const day = padDatePart(value.getDate());

  return `${year}-${month}-${day}`;
};

export const addDaysToDateOnly = (value: string | Date, days: number): Date => {
  const date = value instanceof Date ? new Date(value) : parseDateOnly(value);
  date.setDate(date.getDate() + days);
  return date;
};