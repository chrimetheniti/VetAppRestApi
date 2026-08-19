import {useEffect} from "react";
import {getOwner, updateOwner} from "@/api/owners.ts";
import {useForm} from "react-hook-form";
import {type OwnerUpdateFormFields, ownerUpdateFormSchema} from "@/schemas/owners.ts";
import {zodResolver} from "@hookform/resolvers/zod";
import {toast} from "sonner";
import {Input} from "@/components/ui/input.tsx";
import {Field, FieldLabel} from "@/components/ui/field.tsx";
import {Button} from "@/components/ui/button.tsx";

interface Props {
  ownerId: number;
}

const MyOwnerProfilePage = ({ownerId}: Props) => {
  const {
    register,
    handleSubmit,
    formState: {errors, isSubmitting},
    reset,
  } = useForm<OwnerUpdateFormFields>({
    resolver: zodResolver(ownerUpdateFormSchema),
    defaultValues: {
      email: "",
      firstname: "",
      lastname: "",
      phoneNumber: "",
      address: "",
    },
  });

  useEffect(() => {
    getOwner(ownerId)
      .then((data) => {
        reset({
          email: data.email ?? "",
          firstname: data.firstname ?? "",
          lastname: data.lastname ?? "",
          phoneNumber: data.phoneNumber ?? "",
          address: data.address ?? "",
        });
      })
      .catch(() => toast.error("Failed to load your profile"));
  }, [ownerId, reset]);

  const onSubmit = async (data: OwnerUpdateFormFields) => {
    try {
      await updateOwner(ownerId, data);
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
        {isSubmitting ? "Saving..." : "Save Changes"}
      </Button>
    </form>
  );
};

export default MyOwnerProfilePage;
