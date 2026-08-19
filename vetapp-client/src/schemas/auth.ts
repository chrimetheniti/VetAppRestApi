import {z} from "zod";

export const loginSchema = z.object({
  username: z.string().min(1, {error: "Username is required"}),
  password: z.string().min(1, {error: "Password is required"}),
})

export type LoginFields = z.infer<typeof loginSchema>

export type LoginResponse = {
  token: string;
}

// Owner signup — mirrors OwnerSignupDTO validation rules from the backend.
// Password regex matches backend exactly:
// at least one uppercase, one lowercase, one digit, one special char, min 8 chars.
export const ownerSignupSchema = z.object({
  username: z.string()
      .min(2, {error: "Username must be between 2 and 50 characters"})
      .max(50, {error: "Username must be between 2 and 50 characters"}),
  email: z.email({error: "Invalid email address"})
      .max(100, {error: "Email must not exceed 100 characters"}),
  password: z.string().regex(
      /(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,}$/,
      {error: "Password must contain at least one uppercase, one lowercase, one digit, and one special character"}
  ),
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

export type OwnerSignupFields = z.infer<typeof ownerSignupSchema>

export type OwnerSignupResponse = {
  id: number;
  username: string;
  email: string;
}