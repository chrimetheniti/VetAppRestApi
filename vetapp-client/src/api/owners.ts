import type {Owner} from "@/schemas/owners.ts"
import type {PaginatedResult} from "@/schemas/patients.ts"
import {getCookie} from "@/utils/cookies.ts"

const API_URL = import.meta.env.VITE_API_URL

export async function getOwners(
  pageNumber: number = 1,
  pageSize: number = 100
): Promise<PaginatedResult<Owner>> {
  const token = getCookie("access_token")
  const res = await fetch(
    `${API_URL}/owners?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    {
      headers: {
        "Authorization": `Bearer ${token}`,
      },
    }
  )
  if (!res.ok) throw new Error("Failed to fetch owners")
  return await res.json()
}
