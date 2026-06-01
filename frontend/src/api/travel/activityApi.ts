import { AUTH_TOKEN_KEY } from "../../helpers/authStorage";
import { createApiClient } from "../client";
import type {
  ActivityDto,
  CreateActivityDto,
} from "../../models/travel/activity/dtos";

const activityClient = createApiClient({
  resourcePath: "api/travel",
  getToken: () => localStorage.getItem(AUTH_TOKEN_KEY),
});

export const activityApi = {
  addActivity: async (
    travelId: string,
    destinationId: string,
    data: CreateActivityDto,
  ): Promise<ActivityDto> => {
    const response = await activityClient.post<ActivityDto>(
      `/travels/${travelId}/destinations/${destinationId}/activities`,
      data,
    );

    return response.data;
  },

  updateActivity: async (
    travelId: string,
    destinationId: string,
    id: string,
    data: CreateActivityDto,
  ): Promise<ActivityDto> => {
    const response = await activityClient.put<ActivityDto>(
      `/travels/${travelId}/destinations/${destinationId}/activities/${id}`,
      data,
    );

    return response.data;
  },

  getActivityById: async (
    travelId: string,
    destinationId: string,
    id: string,
  ): Promise<ActivityDto> => {
    const response = await activityClient.get<ActivityDto>(
      `/travels/${travelId}/destinations/${destinationId}/activities/${id}`,
    );

    return response.data;
  },

  getActivitiesByDestinationId: async (
    travelId: string,
    destinationId: string,
  ): Promise<ActivityDto[]> => {
    const response = await activityClient.get<ActivityDto[]>(
      `/travels/${travelId}/destinations/${destinationId}/activities`,
    );

    return response.data;
  },

  deleteActivity: async (
    travelId: string,
    destinationId: string,
    id: string,
  ): Promise<void> => {
    await activityClient.delete(
      `/travels/${travelId}/destinations/${destinationId}/activities/${id}`,
    );
  },
};
