import type {Patient, PaginatedResult} from "@/schemas/patients.ts"
import {getCookie} from "@/utils/cookies.ts"

const API_URL = import.meta.env.VITE_API_URL

export async function getPatients(
  pageNumber: number = 1,
  pageSize: number = 10
): Promise<PaginatedResult<Patient>> {
  const token = getCookie("access_token")
  const res = await fetch(
    `${API_URL}/patients?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    {
      headers: {
        "Authorization": `Bearer ${token}`,
      },
    }
  )
  if (!res.ok) throw new Error("Failed to fetch patients")
  return await res.json()
}
