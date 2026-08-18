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

// Form schema for creating/editing a patient.
// Matches PatientInsertDTO / PatientUpdateDTO validation rules from the backend.
// IDs are kept as strings in the form state (shadcn Select gives strings);
// we convert to numbers in the API call before sending.
export const patientFormSchema = z.object({
  name: z.string()
      .min(1, {error: "Name is required"})
      .max(50, {error: "Name must be at most 50 characters"}),
  chipNumber: z.string()
      .regex(/^\d{15}$/, {error: "Chip number must be exactly 15 digits"})
      .optional()
      .or(z.literal("")),
  species: z.string()
      .min(2, {error: "Species must be between 2 and 50 characters"})
      .max(50, {error: "Species must be between 2 and 50 characters"}),
  breed: z.string()
      .min(2, {error: "Breed must be between 2 and 50 characters"})
      .max(50, {error: "Breed must be between 2 and 50 characters"})
      .optional()
      .or(z.literal("")),
  dateOfBirth: z.string().optional().or(z.literal("")),
  veterinarianId: z.string().min(1, {error: "Please select a veterinarian"}),
  ownerId: z.string().min(1, {error: "Please select an owner"}),
})

export type PatientFormFields = z.infer<typeof patientFormSchema>;