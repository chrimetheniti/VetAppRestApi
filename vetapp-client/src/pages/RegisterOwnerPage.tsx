import {Field, FieldLabel} from "@/components/ui/field.tsx";
import {Input} from "@/components/ui/input.tsx";
import {Button} from "@/components/ui/button.tsx";
import {useForm} from "react-hook-form";
import {type OwnerSignupFields, ownerSignupSchema} from "@/schemas/auth.ts";
import {zodResolver} from "@hookform/resolvers/zod";
import {toast} from "sonner";
import {Link, useNavigate} from "react-router";
import {registerOwner} from "@/api/auth.ts";

export default function RegisterOwnerPage() {
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: {errors, isSubmitting}
  } = useForm<OwnerSignupFields>({
    resolver: zodResolver(ownerSignupSchema)
  })

  const onSubmit = async (data: OwnerSignupFields) => {
    try {
      await registerOwner(data);
      toast.success("Registration successful! Please log in.");
      navigate("/login");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Registration failed");
    }
  }

  return (
    <>
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="max-w-md mx-auto p-8 space-y-6 border rounded bg-white shadow"
      >
        <h1 className="text-2xl font-bold text-center mb-4">Register as Owner</h1>

        <Field>
          <FieldLabel htmlFor="username">Username</FieldLabel>
          <Input id="username" {...register("username")}/>
          {errors.username && (
            <div className="text-destructive text-sm">{errors.username.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="email">Email</FieldLabel>
          <Input id="email" type="email" {...register("email")}/>
          {errors.email && (
            <div className="text-destructive text-sm">{errors.email.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="password">Password</FieldLabel>
          <Input id="password" type="password" {...register("password")}/>
          {errors.password && (
            <div className="text-destructive text-sm">{errors.password.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="firstname">Firstname</FieldLabel>
          <Input id="firstname" {...register("firstname")}/>
          {errors.firstname && (
            <div className="text-destructive text-sm">{errors.firstname.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="lastname">Lastname</FieldLabel>
          <Input id="lastname" {...register("lastname")}/>
          {errors.lastname && (
            <div className="text-destructive text-sm">{errors.lastname.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="phoneNumber">Phone number (optional)</FieldLabel>
          <Input id="phoneNumber" {...register("phoneNumber")}/>
          {errors.phoneNumber && (
            <div className="text-destructive text-sm">{errors.phoneNumber.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="address">Address (optional)</FieldLabel>
          <Input id="address" {...register("address")}/>
          {errors.address && (
            <div className="text-destructive text-sm">{errors.address.message}</div>
          )}
        </Field>

        <Button type="submit" className="w-full" disabled={isSubmitting}>
          {isSubmitting ? "Registering..." : "Register"}
        </Button>

        <div className="text-sm text-center text-muted-foreground">
          Already have an account? <Link to="/login" className="underline">Log in</Link>
        </div>
      </form>
    </>
  )
}
