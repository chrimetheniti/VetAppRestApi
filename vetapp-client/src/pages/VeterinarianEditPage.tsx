import {Navigate, useNavigate, useParams} from "react-router";
import {useEffect} from "react";
import {getVeterinarian, updateVeterinarian} from "@/api/veterinarians.ts";
import {useForm} from "react-hook-form";
import {type VetUpdateFormFields, vetUpdateFormSchema} from "@/schemas/veterinarians.ts";
import {zodResolver} from "@hookform/resolvers/zod";
import {toast} from "sonner";
import {Input} from "@/components/ui/input.tsx";
import {Field, FieldLabel} from "@/components/ui/field.tsx";
import {Button} from "@/components/ui/button.tsx";
import {useAuth} from "@/context/AuthProvider.tsx";

const VeterinarianEditPage = () => {
  const {vetId} = useParams();
  const id = Number(vetId);
  const navigate = useNavigate();
  const {user} = useAuth();
  const canManage = user?.role === "ADMIN" || user?.role === "RECEPTIONIST";

  const {
    register,
    handleSubmit,
    formState: {errors, isSubmitting},
    reset,
  } = useForm<VetUpdateFormFields>({
    resolver: zodResolver(vetUpdateFormSchema),
    defaultValues: {
      email: "",
      firstname: "",
      lastname: "",
      phoneNumber: "",
      clinic: "",
    },
  });

  useEffect(() => {
    if (!canManage || !vetId) return;

    getVeterinarian(id)
      .then((data) => {
        reset({
          email: data.email ?? "",
          firstname: data.firstname ?? "",
          lastname: data.lastname ?? "",
          phoneNumber: data.phoneNumber ?? "",
          clinic: data.clinic ?? "",
        });
      })
      .catch(() => toast.error("Failed to load veterinarian"));
  }, [canManage, vetId, id, reset]);

  if (!canManage) return <Navigate to="/dashboard" replace/>;

  const onSubmit = async (data: VetUpdateFormFields) => {
    try {
      await updateVeterinarian(id, data);
      toast.success("Veterinarian updated successfully");
      navigate("/veterinarians");
    } catch (err) {
      toast.error("Error updating veterinarian");
      console.error(err);
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="max-w-xl mx-auto p-8 border rounded-md space-y-4 bg-white"
      autoComplete="off"
    >
      <h1 className="text-xl font-bold mb-4">Edit Veterinarian</h1>

      <Field>
        <FieldLabel htmlFor="email">Email</FieldLabel>
        <Input id="email" type="email" {...register("email")}/>
        {errors.email && (
          <div className="text-destructive text-sm">{errors.email.message}</div>
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
        {isSubmitting ? "Submitting..." : "Update"}
      </Button>
    </form>
  );
};

export default VeterinarianEditPage;
