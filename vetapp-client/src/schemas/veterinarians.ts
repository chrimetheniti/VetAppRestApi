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

// Form schema for admin-created new vet accounts.
// Matches VeterinarianSignupDTO — phone AND clinic are required (unlike Owner).
export const vetCreateFormSchema = z.object({
  username: z.string()
      .min(2, {error: "Username must be between 2 and 50 characters"})
      .max(50, {error: "Username must be between 2 and 50 characters"}),
  email: z.email({error: "Invalid email address"})
      .max(100, {error: "Email must not exceed 100 characters"}),
  password: z.string()
      .regex(/(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,}$/, {
        error: "Password must contain at least one uppercase, one lowercase, one digit, and one special character (min 8 chars)",
      }),
  firstname: z.string()
      .min(2, {error: "Firstname must be between 2 and 50 characters"})
      .max(50, {error: "Firstname must be between 2 and 50 characters"}),
  lastname: z.string()
      .min(2, {error: "Lastname must be between 2 and 50 characters"})
      .max(50, {error: "Lastname must be between 2 and 50 characters"}),
  phoneNumber: z.string()
      .min(10, {error: "Phone number must be between 10 and 15 characters"})
      .max(15, {error: "Phone number must be between 10 and 15 characters"}),
  clinic: z.string()
      .min(2, {error: "Clinic must be between 2 and 100 characters"})
      .max(100, {error: "Clinic must be between 2 and 100 characters"}),
})
export type VetCreateFormFields = z.infer<typeof vetCreateFormSchema>;

// Form schema for editing a vet (admin editing anyone, or vet editing self).
// Matches VeterinarianUpdateDTO — no username/password change here.
export const vetUpdateFormSchema = z.object({
  email: z.email({error: "Invalid email address"})
      .max(100, {error: "Email must not exceed 100 characters"}),
  firstname: z.string()
      .min(2, {error: "Firstname must be between 2 and 50 characters"})
      .max(50, {error: "Firstname must be between 2 and 50 characters"}),
  lastname: z.string()
      .min(2, {error: "Lastname must be between 2 and 50 characters"})
      .max(50, {error: "Lastname must be between 2 and 50 characters"}),
  phoneNumber: z.string()
      .min(10, {error: "Phone number must be between 10 and 15 characters"})
      .max(15, {error: "Phone number must be between 10 and 15 characters"}),
  clinic: z.string()
      .min(2, {error: "Clinic must be between 2 and 100 characters"})
      .max(100, {error: "Clinic must be between 2 and 100 characters"}),
})
export type VetUpdateFormFields = z.infer<typeof vetUpdateFormSchema>;