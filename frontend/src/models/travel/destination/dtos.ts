export type DestinationDto = {
  id: string;
  travelId: string;
  name: string;
  country: string;
  city: string;
  latitude: number | null;
  longitude: number | null;
  daysSpent: number;
  description: string;
  notes: string;
  activityCount: number;
};

export type CreateDestinationDto = {
  name: string;
  country: string;
  city: string;
  latitude: number | null;
  longitude: number | null;
  daysSpent: number;
  description: string;
  notes: string;
};