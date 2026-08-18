import {z} from "zod";

// Matches OwnerReadOnlyDTO from the backend.
export const ownerSchema = z.object({
  id: z.number().int(),
  userId: z.number().int(),
  username: z.string(),
  email: z.string(),
  firstname: z.string(),
  lastname: z.string(),
  phoneNumber: z.string().nullable().optional(),
  address: z.string().nullable().optional(),
  userRole: z.string(),
})

export type Owner = z.infer<typeof ownerSchema>;
