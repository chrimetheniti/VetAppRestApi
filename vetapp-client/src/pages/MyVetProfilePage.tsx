import {useEffect} from "react";
import {getVeterinarian, updateVeterinarian} from "@/api/veterinarians.ts";
import {useForm} from "react-hook-form";
import {type VetUpdateFormFields, vetUpdateFormSchema} from "@/schemas/veterinarians.ts";
import {zodResolver} from "@hookform/resolvers/zod";
import {toast} from "sonner";
import {Input} from "@/components/ui/input.tsx";
import {Field, FieldLabel} from "@/components/ui/field.tsx";
import {Button} from "@/components/ui/button.tsx";

interface Props {
  vetId: number;
}

const MyVetProfilePage = ({vetId}: Props) => {
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
    getVeterinarian(vetId)
      .then((data) => {
        reset({
          email: data.email ?? "",
          firstname: data.firstname ?? "",
          lastname: data.lastname ?? "",
          phoneNumber: data.phoneNumber ?? "",
          clinic: data.clinic ?? "",
        });
      })
      .catch(() => toast.error("Failed to load your profile"));
  }, [vetId, reset]);

  const onSubmit = async (data: VetUpdateFormFields) => {
    try {
      await updateVeterinarian(vetId, data);
      toast.success("Profile updated successfully");
    } catch (err) {
      toast.error("Error updating profile");
      console.error(err);
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="max-w-xl mx-auto p-8 border rounded-md space-y-4 bg-white"
      autoComplete="off"
    >
      <h1 className="text-xl font-bold mb-4">My Profile</h1>

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
        {isSubmitting ? "Saving..." : "Save Changes"}
      </Button>
    </form>
  );
};

export default MyVetProfilePage;
