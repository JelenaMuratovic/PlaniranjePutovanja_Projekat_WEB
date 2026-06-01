export type TravelDto = {
  id: string;
  userId: string;
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  budget: number;
  notes: string;
  createdAt: string;
  destinationCount: number;
};

export type CreateTravelDto = {
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  budget: number;
  notes: string;
};

export type UpdateTravelDto = CreateTravelDto;