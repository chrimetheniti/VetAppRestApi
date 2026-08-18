import type {Veterinarian} from "@/schemas/veterinarians.ts"
import type {PaginatedResult} from "@/schemas/patients.ts"
import {getCookie} from "@/utils/cookies.ts"

const API_URL = import.meta.env.VITE_API_URL

export async function getVeterinarians(
  pageNumber: number = 1,
  pageSize: number = 100
): Promise<PaginatedResult<Veterinarian>> {
  const token = getCookie("access_token")
  const res = await fetch(
    `${API_URL}/veterinarians?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    {
      headers: {
        "Authorization": `Bearer ${token}`,
      },
    }
  )
  if (!res.ok) throw new Error("Failed to fetch veterinarians")
  return await res.json()
}
