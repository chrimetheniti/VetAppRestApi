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

// Form schema for admin-created new owner accounts.
// Matches OwnerSignupDTO validation rules from the backend.
export const ownerCreateFormSchema = z.object({
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
      .max(15, {error: "Phone number must be between 10 and 15 characters"})
      .optional()
      .or(z.literal("")),
  address: z.string()
      .min(5, {error: "Address must be between 5 and 200 characters"})
      .max(200, {error: "Address must be between 5 and 200 characters"})
      .optional()
      .or(z.literal("")),
})
export type OwnerCreateFormFields = z.infer<typeof ownerCreateFormSchema>;

// Form schema for editing an owner (admin editing anyone, or owner editing self).
// Matches OwnerUpdateDTO — no username/password change here.
export const ownerUpdateFormSchema = z.object({
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
      .max(15, {error: "Phone number must be between 10 and 15 characters"})
      .optional()
      .or(z.literal("")),
  address: z.string()
      .min(5, {error: "Address must be between 5 and 200 characters"})
      .max(200, {error: "Address must be between 5 and 200 characters"})
      .optional()
      .or(z.literal("")),
})
export type OwnerUpdateFormFields = z.infer<typeof ownerUpdateFormSchema>;