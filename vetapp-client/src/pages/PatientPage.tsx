import {useNavigate, useParams} from "react-router";
import {useEffect, useState} from "react";
import {createPatient, getPatient, updatePatient} from "@/api/patients.ts";
import {getVeterinarians} from "@/api/veterinarians.ts";
import {getOwners} from "@/api/owners.ts";
import {useForm} from "react-hook-form";
import {type PatientFormFields, patientFormSchema} from "@/schemas/patients.ts";
import type {Veterinarian} from "@/schemas/veterinarians.ts";
import type {Owner} from "@/schemas/owners.ts";
import {zodResolver} from "@hookform/resolvers/zod";
import {toast} from "sonner";
import {Input} from "@/components/ui/input.tsx";
import {Field, FieldLabel} from "@/components/ui/field.tsx";
import {Button} from "@/components/ui/button.tsx";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select.tsx";

const PatientPage = () => {
  const {patientId} = useParams();
  const id = Number(patientId);
  const isEdit = !!patientId;
  const navigate = useNavigate();

  const [vets, setVets] = useState<Veterinarian[]>([]);
  const [owners, setOwners] = useState<Owner[]>([]);

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: {errors, isSubmitting},
    reset,
  } = useForm<PatientFormFields>({
    resolver: zodResolver(patientFormSchema),
    defaultValues: {
      name: "",
      chipNumber: "",
      species: "",
      breed: "",
      dateOfBirth: "",
      veterinarianId: "",
      ownerId: "",
    },
  });

  // Load dropdown options (vets + owners) once on mount.
  useEffect(() => {
    getVeterinarians()
      .then((result) => setVets(result.data))
      .catch(() => toast.error("Failed to load veterinarians"));

    getOwners()
      .then((result) => setOwners(result.data))
      .catch(() => toast.error("Failed to load owners"));
  }, []);

  // On edit: fetch the patient and populate the form.
  useEffect(() => {
    if (isEdit && patientId) {
      getPatient(id)
        .then((data) => {
          const values: PatientFormFields = {
            name: data.name ?? "",
            chipNumber: data.chipNumber ?? "",
            species: data.species ?? "",
            breed: data.breed ?? "",
            // Backend returns "YYYY-MM-DDTHH:mm:ss"; input type=date wants "YYYY-MM-DD".
            dateOfBirth: data.dateOfBirth ? data.dateOfBirth.split("T")[0] : "",
            veterinarianId: data.veterinarianId.toString(),
            ownerId: data.ownerId.toString(),
          };
          reset(values);
        })
        .catch(() => toast.error("Failed to load patient"));
    }
  }, [isEdit, patientId, id, reset]);

  const onSubmit = async (data: PatientFormFields) => {
    try {
      if (isEdit) {
        await updatePatient(id, data);
        toast.success("Patient updated successfully");
      } else {
        await createPatient(data);
        toast.success("Patient created successfully");
      }
      navigate("/patients");
    } catch (err) {
      toast.error(isEdit ? "Error updating patient" : "Error creating patient");
      console.error(err);
    }
  };

  return (
    <>
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="max-w-xl mx-auto p-8 border rounded-md space-y-4 bg-white"
        autoComplete="off"
      >
        <h1 className="text-xl font-bold mb-4">
          {isEdit ? "Edit Patient" : "New Patient"}
        </h1>

        <Field>
          <FieldLabel htmlFor="name">Name</FieldLabel>
          <Input id="name" {...register("name")}/>
          {errors.name && (
            <div className="text-destructive text-sm">{errors.name.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="species">Species</FieldLabel>
          <Input id="species" {...register("species")}/>
          {errors.species && (
            <div className="text-destructive text-sm">{errors.species.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="breed">Breed (optional)</FieldLabel>
          <Input id="breed" {...register("breed")}/>
          {errors.breed && (
            <div className="text-destructive text-sm">{errors.breed.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="chipNumber">Chip number (optional, 15 digits)</FieldLabel>
          <Input id="chipNumber" {...register("chipNumber")}/>
          {errors.chipNumber && (
            <div className="text-destructive text-sm">{errors.chipNumber.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="dateOfBirth">Date of birth (optional)</FieldLabel>
          <Input id="dateOfBirth" type="date" {...register("dateOfBirth")}/>
          {errors.dateOfBirth && (
            <div className="text-destructive text-sm">{errors.dateOfBirth.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="veterinarianId">Veterinarian</FieldLabel>
          <Select
            value={watch("veterinarianId")}
            onValueChange={(v) => setValue("veterinarianId", v)}
          >
            <SelectTrigger id="veterinarianId">
              <SelectValue placeholder="Select a veterinarian"/>
            </SelectTrigger>
            <SelectContent>
              {vets.map((vet) => (
                <SelectItem key={vet.id} value={vet.id.toString()}>
                  {vet.firstname} {vet.lastname} — {vet.clinic}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          {errors.veterinarianId && (
            <div className="text-destructive text-sm">{errors.veterinarianId.message}</div>
          )}
        </Field>

        <Field>
          <FieldLabel htmlFor="ownerId">Owner</FieldLabel>
          <Select
            value={watch("ownerId")}
            onValueChange={(v) => setValue("ownerId", v)}
          >
            <SelectTrigger id="ownerId">
              <SelectValue placeholder="Select an owner"/>
            </SelectTrigger>
            <SelectContent>
              {owners.map((owner) => (
                <SelectItem key={owner.id} value={owner.id.toString()}>
                  {owner.firstname} {owner.lastname}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          {errors.ownerId && (
            <div className="text-destructive text-sm">{errors.ownerId.message}</div>
          )}
        </Field>

        <Button type="submit" disabled={isSubmitting} className="w-full">
          {isSubmitting ? "Submitting..." : isEdit ? "Update" : "Create"}
        </Button>
      </form>
    </>
  );
};

export default PatientPage;
