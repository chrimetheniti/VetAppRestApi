import {z} from "zod";

// Matches VeterinarianReadOnlyDTO from the backend.
export const veterinarianSchema = z.object({
  id: z.number().int(),
  userId: z.number().int(),
  username: z.string(),
  email: z.string(),
  firstname: z.string(),
  lastname: z.string(),
  clinic: z.string(),
  phoneNumber: z.string().nullable().optional(),
  userRole: z.string(),
})

export type Veterinarian = z.infer<typeof veterinarianSchema>;
