import { AUTH_TOKEN_KEY } from "../../helpers/authStorage";
import { createApiClient } from "../client";
import type {
  GenerateShareLinkRequestDto,
  ShareLinkResponseDto,
} from "../../models/util/dtos";

export const utilClient = createApiClient({
  resourcePath: "api/util",
  getToken: () => localStorage.getItem(AUTH_TOKEN_KEY),
});

export const utilApi = {
  generateShare: async (
    travelId: string,
    data: GenerateShareLinkRequestDto,
  ): Promise<ShareLinkResponseDto> => {
    const response = await utilClient.post<ShareLinkResponseDto>(
      `/travels/${travelId}/share`,
      data,
    );
    return response.data;
  },

  getSharedTravel: async (token: string): Promise<any> => {
    const response = await utilClient.get(`/shared?token=${token}`);
    return response.data;
  },

  /**
   * Preuzima PDF fajl sa plana putovanja kao Blob (Binary Large Object)
   */
  exportPdf: async (travelId: string): Promise<Blob> => {
    const response = await utilClient.get(`/travels/${travelId}/pdf`, {
      responseType: "blob", // Govori Axiosu da ocekujemo binarni fajl (PDF), a ne JSON/String
    });
    return response.data;
  },
};
