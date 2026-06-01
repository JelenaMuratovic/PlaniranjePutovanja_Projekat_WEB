import { AUTH_TOKEN_KEY } from "../../helpers/authStorage";
import { createApiClient } from "../client";
import type {
  ChecklistDto,
  CreateChecklistDto,
} from "../../models/travel/checklist/dtos";

const checklistClient = createApiClient({
  resourcePath: "api/travel",
  getToken: () => localStorage.getItem(AUTH_TOKEN_KEY),
});

export const checklistApi = {
  addChecklist: async (
    travelId: string,
    data: CreateChecklistDto,
  ): Promise<ChecklistDto> => {
    const response = await checklistClient.post<ChecklistDto>(
      `/travels/${travelId}/checklists`,
      data,
    );

    return response.data;
  },

  getChecklistById: async (
    travelId: string,
    id: string,
  ): Promise<ChecklistDto> => {
    const response = await checklistClient.get<ChecklistDto>(
      `/travels/${travelId}/checklists/${id}`,
    );

    return response.data;
  },

  getChecklistsByTravelId: async (
    travelId: string,
  ): Promise<ChecklistDto[]> => {
    const response = await checklistClient.get<ChecklistDto[]>(
      `/travels/${travelId}/checklists`,
    );

    return response.data;
  },

  toggleChecklist: async (
    travelId: string,
    id: string,
    isCompleted: boolean,
  ): Promise<ChecklistDto> => {
    const response = await checklistClient.put<ChecklistDto>(
      `/travels/${travelId}/checklists/${id}/toggle`,
      null,
      {
        params: { isCompleted },
      },
    );

    return response.data;
  },

  deleteChecklist: async (travelId: string, id: string): Promise<void> => {
    await checklistClient.delete(`/travels/${travelId}/checklists/${id}`);
  },
};