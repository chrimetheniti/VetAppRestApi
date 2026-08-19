import type {Owner, OwnerCreateFormFields, OwnerUpdateFormFields} from "@/schemas/owners.ts"
import type {PaginatedResult} from "@/schemas/patients.ts"
import {getCookie} from "@/utils/cookies.ts"

const API_URL = import.meta.env.VITE_API_URL

// OWNER role id — matches backend seed (Roles table).
const OWNER_ROLE_ID = 4;

export async function getOwners(
    pageNumber: number = 1,
    pageSize: number = 10
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

export async function getOwner(id: number): Promise<Owner> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/owners/${id}`, {
        headers: {
            "Authorization": `Bearer ${token}`,
        },
    })
    if (!res.ok) throw new Error("Failed to fetch owner")
    return await res.json()
}

// Owner creation goes through the public /auth/register/owner endpoint.
// Admin-initiated creation uses the same endpoint — we just send the token
// too so authenticated context is preserved.
export async function createOwner(fields: OwnerCreateFormFields): Promise<{ id: number }> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/auth/register/owner`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            ...(token ? {"Authorization": `Bearer ${token}`} : {}),
        },
        body: JSON.stringify({...buildCreatePayload(fields), roleId: OWNER_ROLE_ID}),
    })
    if (!res.ok) throw new Error("Failed to create owner")
    return await res.json()
}

export async function updateOwner(id: number, fields: OwnerUpdateFormFields): Promise<Owner> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/owners/${id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify({id, ...buildUpdatePayload(fields)}),
    })
    if (!res.ok) throw new Error("Failed to update owner")
    return await res.json()
}

export async function deleteOwner(id: number): Promise<void> {
    const token = getCookie("access_token")
    const res = await fetch(`${API_URL}/owners/${id}`, {
        method: "DELETE",
        headers: {
            "Authorization": `Bearer ${token}`,
        },
    })
    if (!res.ok) throw new Error("Failed to delete owner")
}

// Empty strings for optional fields → undefined (JSON omits them; backend allows null).
function buildCreatePayload(fields: OwnerCreateFormFields) {
    return {
        username: fields.username,
        email: fields.email,
        password: fields.password,
        firstname: fields.firstname,
        lastname: fields.lastname,
        phoneNumber: fields.phoneNumber || undefined,
        address: fields.address || undefined,
    }
}

function buildUpdatePayload(fields: OwnerUpdateFormFields) {
    return {
        email: fields.email,
        firstname: fields.firstname,
        lastname: fields.lastname,
        phoneNumber: fields.phoneNumber || undefined,
        address: fields.address || undefined,
    }
}