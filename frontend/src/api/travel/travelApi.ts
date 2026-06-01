import { AUTH_TOKEN_KEY } from "../../helpers/authStorage";
import { createApiClient } from "../client";
import type {
  CreateTravelDto,
  TravelDto,
  UpdateTravelDto,
} from "../../models/travel/travel/dtos";

const travelClient = createApiClient({
  resourcePath: "api/travel",
  getToken: () => localStorage.getItem(AUTH_TOKEN_KEY),
});

export const travelApi = {
  createTravel: async (data: CreateTravelDto): Promise<TravelDto> => {
    const response = await travelClient.post<TravelDto>("/travels", data);
    return response.data;
  },

  updateTravel: async (
    id: string,
    data: UpdateTravelDto,
  ): Promise<TravelDto> => {
    const response = await travelClient.put<TravelDto>(`/travels/${id}`, data);
    return response.data;
  },

  getTravelById: async (id: string): Promise<TravelDto> => {
    const response = await travelClient.get<TravelDto>(`/travels/${id}`);
    return response.data;
  },

  getTravelsByUserId: async (userId: string): Promise<TravelDto[]> => {
    const response = await travelClient.get<TravelDto[]>(
      `/travels/user/${userId}`,
    );
    return response.data;
  },

  getAllTravels: async (): Promise<TravelDto[]> => {
    const response = await travelClient.get<TravelDto[]>("/travels");
    return response.data;
  },

  deleteTravel: async (id: string): Promise<void> => {
    await travelClient.delete(`/travels/${id}`);
  },
};
