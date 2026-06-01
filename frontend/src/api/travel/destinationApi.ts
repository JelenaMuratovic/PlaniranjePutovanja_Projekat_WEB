import { AUTH_TOKEN_KEY } from "../../helpers/authStorage";
import { createApiClient } from "../client";
import type {
  CreateDestinationDto,
  DestinationDto,
} from "../../models/travel/destination/dtos";

const destinationClient = createApiClient({
  resourcePath: "api/travel",
  getToken: () => localStorage.getItem(AUTH_TOKEN_KEY),
});

export const destinationApi = {
  addDestination: async (
    travelId: string,
    data: CreateDestinationDto,
  ): Promise<DestinationDto> => {
    const response = await destinationClient.post<DestinationDto>(
      `/travels/${travelId}/destinations`,
      data,
    );

    return response.data;
  },

  updateDestination: async (
    travelId: string,
    id: string,
    data: CreateDestinationDto,
  ): Promise<DestinationDto> => {
    const response = await destinationClient.put<DestinationDto>(
      `/travels/${travelId}/destinations/${id}`,
      data,
    );

    return response.data;
  },

  getDestinationById: async (
    travelId: string,
    id: string,
  ): Promise<DestinationDto> => {
    const response = await destinationClient.get<DestinationDto>(
      `/travels/${travelId}/destinations/${id}`,
    );

    return response.data;
  },

  getDestinationsByTravelId: async (
    travelId: string,
  ): Promise<DestinationDto[]> => {
    const response = await destinationClient.get<DestinationDto[]>(
      `/travels/${travelId}/destinations`,
    );

    return response.data;
  },

  deleteDestination: async (travelId: string, id: string): Promise<void> => {
    await destinationClient.delete(`/travels/${travelId}/destinations/${id}`);
  },
};
