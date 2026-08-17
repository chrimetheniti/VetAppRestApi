import {z} from "zod";

// Matches PatientReadOnlyDTO from the backend.
// Denormalized vet + owner info (fullname, clinic, phone) is included
// for easy display in list/detail views without extra API calls.
export const patientSchema = z.object({
  id: z.number().int(),
  name: z.string(),
  chipNumber: z.string().nullable().optional(),
  species: z.string(),
  breed: z.string().nullable().optional(),
  dateOfBirth: z.string().nullable().optional(),
  veterinarianId: z.number().int(),
  veterinarianFullName: z.string(),
  veterinarianClinic: z.string(),
  ownerId: z.number().int(),
  ownerFullName: z.string(),
  ownerPhoneNumber: z.string().nullable().optional(),
})

export type Patient = z.infer<typeof patientSchema>;

// Matches PaginatedResult<T> wrapper the backend returns for list endpoints.
export type PaginatedResult<T> = {
  data: T[];
  totalRecords: number;
  pageNumber: number;
  pageSize: number;
}
