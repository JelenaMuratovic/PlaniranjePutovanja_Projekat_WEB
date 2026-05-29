import type { UserDto } from "../models/auth/dtos";

type JwtPayload = {
  sub?: string;
  nameid?: string;
  email?: string;
  unique_name?: string;
  name?: string;
  role?: string | string[];
  roles?: string | string[];
  exp?: number;
  [key: string]: unknown;
};

type DecodedJwtUser = UserDto & {
  displayName: string;
  expiresAt?: number;
};

const decodeBase64Url = (value: string): string => {
  const normalized = value.replace(/-/g, "+").replace(/_/g, "/");
  const padded = normalized.padEnd(Math.ceil(normalized.length / 4) * 4, "=");

  return globalThis.atob(padded);
};

const getFirstClaimValue = (claim?: string | string[]): string => {
  if (!claim) {
    return "";
  }

  return Array.isArray(claim) ? (claim[0] ?? "") : claim;
};

const splitDisplayName = (
  displayName: string,
): { firstName: string; lastName: string } => {
  const trimmed = displayName.trim();

  if (!trimmed) {
    return { firstName: "", lastName: "" };
  }

  const parts = trimmed.split(/\s+/);

  return {
    firstName: parts[0] ?? "",
    lastName: parts.slice(1).join(" "),
  };
};

export const decodeJwt = (token: string): DecodedJwtUser | null => {
  try {
    const payloadPart = token.split(".")[1];

    if (!payloadPart) {
      return null;
    }

    const payload = JSON.parse(decodeBase64Url(payloadPart)) as JwtPayload;
    const displayName =
      payload.name?.toString() ?? payload.unique_name?.toString() ?? "";
    const { firstName, lastName } = splitDisplayName(displayName);
    const role = getFirstClaimValue(payload.role ?? payload.roles);
    const id = payload.sub?.toString() ?? payload.nameid?.toString() ?? "";
    const email = payload.email?.toString() ?? "";

    return {
      id,
      firstName,
      lastName,
      email,
      role,
      displayName,
      expiresAt: payload.exp,
    };
  } catch {
    return null;
  }
};

export const isJwtExpired = (token: string): boolean => {
  const decoded = decodeJwt(token);

  if (!decoded?.expiresAt) {
    return true;
  }

  return decoded.expiresAt * 1000 <= Date.now();
};
