type TravelDayStripProps = {
  startDate: string;
  endDate: string;
};

const getDateOnlyPart = (value: string): string => value.split("T")[0] ?? value;

const parseDateOnly = (value: string): Date => {
  const [yearPart, monthPart, dayPart] = getDateOnlyPart(value).split("-");
  const year = Number(yearPart);
  const month = Number(monthPart);
  const day = Number(dayPart);

  if (!year || !month || !day) {
    return new Date(value);
  }

  return new Date(year, month - 1, day);
};

const calculateTripDays = (startDate: string, endDate: string): number => {
  const start = parseDateOnly(startDate);
  const end = parseDateOnly(endDate);

  if (Number.isNaN(start.getTime()) || Number.isNaN(end.getTime())) {
    return 0;
  }

  const millisPerDay = 1000 * 60 * 60 * 24;
  const diff = Math.max(
    0,
    Math.floor((end.getTime() - start.getTime()) / millisPerDay),
  );

  return diff + 1;
};

export const TravelDayStrip = ({ startDate, endDate }: TravelDayStripProps) => {
  const totalDays = calculateTripDays(startDate, endDate);

  if (totalDays <= 0) {
    return null;
  }

  return (
    <div className="travel-day-strip" aria-label="Travel days overview">
      {Array.from({ length: totalDays }, (_, index) => index + 1).map(
        (dayNumber) => (
          <div key={dayNumber} className="travel-day-strip__day">
            <span className="travel-day-strip__label">Day</span>
            <strong>{dayNumber}</strong>
          </div>
        ),
      )}
    </div>
  );
};

export { calculateTripDays };
