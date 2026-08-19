import type {Veterinarian, VetCreateFormFields, VetUpdateFormFields} from "@/schemas/veterinarians.ts"
import type {PaginatedResult} from "@/schemas/patients.ts"
import {getCookie} from "@/utils/cookies.ts"

const API_URL = import.meta.env.VITE_API_URL

// VETERINARIAN role id — matches backend seed (Roles table).
const VETERINARIAN_ROLE_ID = 3;

export async function getVeterinarians(
    pageNumber: number = 1,
    pageSize: number = 10
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

export async function getVeterinarian(id: number): Promise<Veterinarian> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/veterinarians/${id}`, {
        headers: {
            "Authorization": `Bearer ${token}`,
        },
    })
    if (!res.ok) throw new Error("Failed to fetch veterinarian")
    return await res.json()
}

// Vet creation goes through the public /auth/register/veterinarian endpoint.
export async function createVeterinarian(fields: VetCreateFormFields): Promise<{ id: number }> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/auth/register/veterinarian`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            ...(token ? {"Authorization": `Bearer ${token}`} : {}),
        },
        body: JSON.stringify({...fields, roleId: VETERINARIAN_ROLE_ID}),
    })
    if (!res.ok) throw new Error("Failed to create veterinarian")
    return await res.json()
}

export async function updateVeterinarian(id: number, fields: VetUpdateFormFields): Promise<Veterinarian> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/veterinarians/${id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify({id, ...fields}),
    })
    if (!res.ok) throw new Error("Failed to update veterinarian")
    return await res.json()
}

export async function deleteVeterinarian(id: number): Promise<void> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/veterinarians/${id}`, {
        method: "DELETE",
        headers: {
            "Authorization": `Bearer ${token}`,
        },
    })
    if (!res.ok) throw new Error("Failed to delete veterinarian")
}