import type {LoginFields, LoginResponse} from "@/schemas/auth.ts";
import {createContext, useContext, useState} from "react";
import {jwtDecode} from "jwt-decode";
import {deleteCookie, getCookie, setCookie} from "@/utils/cookies.ts";
import {login} from "@/api/auth.ts";

type Role = "ADMIN" | "RECEPTIONIST" | "VETERINARIAN" | "OWNER";

type UserInfo = {
  userId: number;
  username: string;
  email: string;
  role: Role;
  veterinarianId?: number;
  ownerId?: number;
}

// .NET emits standard claims under ClaimTypes URIs;
// custom claims (veterinarianId, ownerId) use plain keys.
// noinspection HttpUrlsUsage
type JwtPayload = {
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": Role;
  veterinarianId?: string;
  ownerId?: string;
  exp: number;
}

// noinspection HttpUrlsUsage
const CLAIM = {
  NAME_IDENTIFIER: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
  NAME: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
  EMAIL: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
  ROLE: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
} as const;

type AuthContextProps = {
  isAuthenticated: boolean;
  accessToken: string | null;
  user: UserInfo | null;
  loginUser: (fields: LoginFields) => Promise<void>;
  logoutUser: () => void;
}

const AuthContext = createContext<AuthContextProps | undefined>(undefined)

function readUserFromToken(token: string | null): UserInfo | null {
  if (!token) return null;
  try {
    const payload = jwtDecode<JwtPayload>(token);
    return {
      userId: Number(payload[CLAIM.NAME_IDENTIFIER]),
      username: payload[CLAIM.NAME],
      email: payload[CLAIM.EMAIL],
      role: payload[CLAIM.ROLE],
      veterinarianId: payload.veterinarianId ? Number(payload.veterinarianId) : undefined,
      ownerId: payload.ownerId ? Number(payload.ownerId) : undefined,
    }
  } catch {
    return null;
  }
}

export const AuthProvider = ({children}: {children: React.ReactNode}) => {
  const cookieAccessToken = getCookie("access_token")
  const [accessToken, setAccessToken] = useState<string | null>(
    () => cookieAccessToken ?? null
  );
  const [user, setUser] = useState<UserInfo | null>(
    readUserFromToken(cookieAccessToken ?? null)
  );

  const loginUser = async (fields: LoginFields) => {
    const res: LoginResponse = await login(fields);
    setCookie("access_token", res.token, {
      expires: 1,
      sameSite: "Lax",
      secure: false,
      path: "/",
    });
    setAccessToken(res.token);
    setUser(readUserFromToken(res.token));
  }

  const logoutUser = () => {
    deleteCookie("access_token");
    setAccessToken(null);
    setUser(null);
  }

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated: !!accessToken,
        accessToken,
        user,
        loginUser,
        logoutUser,
      }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within an AuthProvider");
  return ctx;
}
