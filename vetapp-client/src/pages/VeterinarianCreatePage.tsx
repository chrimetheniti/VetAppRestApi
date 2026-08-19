import {Navigate, useNavigate} from "react-router";
import {createVeterinarian} from "@/api/veterinarians.ts";
import {useForm} from "react-hook-form";
import {type VetCreateFormFields, vetCreateFormSchema} from "@/schemas/veterinarians.ts";
import {zodResolver} from "@hookform/resolvers/zod";
import {toast} from "sonner";
import {Input} from "@/components/ui/input.tsx";
import {Field, FieldLabel} from "@/components/ui/field.tsx";
import {Button} from "@/components/ui/button.tsx";
import {useAuth} from "@/context/AuthProvider.tsx";

const VeterinarianCreatePage = () => {
  const navigate = useNavigate();
  const {user} = useAuth();
  const canManage = user?.role === "ADMIN" || user?.role === "RECEPTIONIST";

  const {
    register,
    handleSubmit,
    formState: {errors, isSubmitting},
  } = useForm<VetCreateFormFields>({
    resolver: zodResolver(vetCreateFormSchema),
    defaultValues: {
      username: "",
      email: "",
      password: "",
      firstname: "",
      lastname: "",
      phoneNumber: "",
      clinic: "",
    },
  });

  if (!canManage) return <Navigate to="/dashboard" replace/>;

  const onSubmit = async (data: VetCreateFormFields) => {
    try {
      await createVeterinarian(data);
      toast.success("Veterinarian created successfully");
      navigate("/veterinarians");
    } catch (err) {
      toast.error("Error creating veterinarian");
      console.error(err);
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="max-w-xl mx-auto p-8 border rounded-md space-y-4 bg-white"
      autoComplete="off"
    >
      <h1 className="text-xl font-bold mb-4">New Veterinarian</h1>

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
        <FieldLabel htmlFor="phoneNumber">Phone</FieldLabel>
        <Input id="phoneNumber" {...register("phoneNumber")}/>
        {errors.phoneNumber && (
          <div className="text-destructive text-sm">{errors.phoneNumber.message}</div>
        )}
      </Field>

      <Field>
        <FieldLabel htmlFor="clinic">Clinic</FieldLabel>
        <Input id="clinic" {...register("clinic")}/>
        {errors.clinic && (
          <div className="text-destructive text-sm">{errors.clinic.message}</div>
        )}
      </Field>

      <Button type="submit" disabled={isSubmitting} className="w-full">
        {isSubmitting ? "Submitting..." : "Create"}
      </Button>
    </form>
  );
};

export default VeterinarianCreatePage;
