import type {Patient, PaginatedResult, PatientFormFields} from "@/schemas/patients.ts"
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

export async function getPatient(id: number): Promise<Patient> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/patients/${id}`, {
        headers: {
            "Authorization": `Bearer ${token}`,
        },
    })
    if (!res.ok) throw new Error("Failed to fetch patient")
    return await res.json()
}

export async function createPatient(fields: PatientFormFields): Promise<Patient> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/patients`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify(buildPayload(fields)),
    })
    if (!res.ok) throw new Error("Failed to create patient")
    return await res.json()
}

export async function updatePatient(id: number, fields: PatientFormFields): Promise<Patient> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/patients/${id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify({ id, ...buildPayload(fields) }),
    })
    if (!res.ok) throw new Error("Failed to update patient")
    return await res.json()
}

export async function deletePatient(id: number): Promise<void> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/patients/${id}`, {
        method: "DELETE",
        headers: {
            "Authorization": `Bearer ${token}`,
        },
    })
    if (!res.ok) throw new Error("Failed to delete patient")
}

// Converts form fields (all strings) to backend payload:
// - Empty strings for optional fields → undefined (so JSON omits them; backend allows null)
// - IDs from Select → numbers
function buildPayload(fields: PatientFormFields) {
    return {
        name: fields.name,
        chipNumber: fields.chipNumber || undefined,
        species: fields.species,
        breed: fields.breed || undefined,
        dateOfBirth: fields.dateOfBirth || undefined,
        veterinarianId: Number(fields.veterinarianId),
        ownerId: Number(fields.ownerId),
    }
}