import type {LoginFields, LoginResponse} from "@/schemas/auth.ts";

const API_URL = import.meta.env.VITE_API_URL

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
