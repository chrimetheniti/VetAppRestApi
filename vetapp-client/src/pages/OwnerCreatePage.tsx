import {Navigate, useNavigate} from "react-router";
import {createOwner} from "@/api/owners.ts";
import {useForm} from "react-hook-form";
import {type OwnerCreateFormFields, ownerCreateFormSchema} from "@/schemas/owners.ts";
import {zodResolver} from "@hookform/resolvers/zod";
import {toast} from "sonner";
import {Input} from "@/components/ui/input.tsx";
import {Field, FieldLabel} from "@/components/ui/field.tsx";
import {Button} from "@/components/ui/button.tsx";
import {useAuth} from "@/context/AuthProvider.tsx";

const OwnerCreatePage = () => {
  const navigate = useNavigate();
  const {user} = useAuth();
  const canManage = user?.role === "ADMIN" || user?.role === "RECEPTIONIST";

  const {
    register,
    handleSubmit,
    formState: {errors, isSubmitting},
  } = useForm<OwnerCreateFormFields>({
    resolver: zodResolver(ownerCreateFormSchema),
    defaultValues: {
      username: "",
      email: "",
      password: "",
      firstname: "",
      lastname: "",
      phoneNumber: "",
      address: "",
    },
  });

  if (!canManage) return <Navigate to="/dashboard" replace/>;

  const onSubmit = async (data: OwnerCreateFormFields) => {
    try {
      await createOwner(data);
      toast.success("Owner created successfully");
      navigate("/owners");
    } catch (err) {
      toast.error("Error creating owner");
      console.error(err);
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="max-w-xl mx-auto p-8 border rounded-md space-y-4 bg-white"
      autoComplete="off"
    >
      <h1 className="text-xl font-bold mb-4">New Owner</h1>

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
        <FieldLabel htmlFor="phoneNumber">Phone (optional)</FieldLabel>
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

      <Button type="submit" disabled={isSubmitting} className="w-full">
        {isSubmitting ? "Submitting..." : "Create"}
      </Button>
    </form>
  );
};

export default OwnerCreatePage;
