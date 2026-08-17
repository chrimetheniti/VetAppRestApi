import type {
  LoginFields,
  LoginResponse,
  OwnerSignupFields,
  OwnerSignupResponse,
} from "@/schemas/auth.ts";

const API_URL = import.meta.env.VITE_API_URL

// Roles seeded in the backend: 1=ADMIN, 2=RECEPTIONIST, 3=VETERINARIAN, 4=OWNER.
const OWNER_ROLE_ID = 4;

export async function login({
                              username,
                              password,
                            }: LoginFields): Promise<LoginResponse> {
  const res = await fetch(API_URL + "/auth/login", {
    method: "POST",
    headers: {"Content-Type": "application/json"},
    body: JSON.stringify({username, password}),
  })

  if (!res.ok) {
    let detail = "Login Failed"
    try {
      const data = await res.json()
      if (typeof data?.detail === "string") detail = data.detail
      else if (typeof data?.title === "string") detail = data.title
    } catch (error) {
      console.error("Error parsing login response", error)
    }
    throw new Error(detail)
  }

  return await res.json()
}

export async function registerOwner(fields: OwnerSignupFields): Promise<OwnerSignupResponse> {
  // Strip empty strings from optional fields so the backend receives
  // undefined instead of "" (which would fail its min-length validation).
  const payload = {
    ...fields,
    phoneNumber: fields.phoneNumber || undefined,
    address: fields.address || undefined,
    roleId: OWNER_ROLE_ID,
  };

  const res = await fetch(API_URL + "/auth/register/owner", {
    method: "POST",
    headers: {"Content-Type": "application/json"},
    body: JSON.stringify(payload),
  })

  if (!res.ok) {
    let detail = "Registration failed"
    try {
      const data = await res.json()
      if (typeof data?.detail === "string") detail = data.detail
      else if (typeof data?.title === "string") detail = data.title
    } catch (error) {
      console.error("Error parsing register response", error)
    }
    throw new Error(detail)
  }

  return await res.json()
}