export type ShareAccessLevel = "View" | "Edit";

export type GeneratedFileDto = {
  fileContents: string; // Na frontu stize kao Base64 string u JSON-u (za QR kod)
  contentType: string; // Npr. "image/png" ili "application/pdf"
  fileName: string;
};

export type GenerateShareLinkRequestDto = {
  accessLevel: ShareAccessLevel | number;
  expirationDays: number;
};

export type ShareLinkResponseDto = {
  token: string;
  targetUrl: string;
  qrCodeImage: GeneratedFileDto;
};
