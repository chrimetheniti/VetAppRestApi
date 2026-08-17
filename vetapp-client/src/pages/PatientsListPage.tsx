import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import {useEffect, useState} from "react";
import {getPatients} from "@/api/patients.ts";
import type {Patient} from "@/schemas/patients.ts";
import {toast} from "sonner";

const PatientsListPage = () => {
  const [patients, setPatients] = useState<Patient[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getPatients()
      .then((result) => setPatients(result.data))
      .catch(() => toast.error("Failed to load patients"))
      .finally(() => setLoading(false))
  }, [])

  if (loading) return <div className="p-8 text-center">Loading...</div>

  return (
    <>
      <div className="p-8">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-2xl font-bold">
            Patients
          </h1>
        </div>
        <Table>
          <TableCaption>A list of all patients.</TableCaption>
          <TableHeader className="bg-gray-100 font-bold">
            <TableRow>
              <TableHead>#</TableHead>
              <TableHead>Name</TableHead>
              <TableHead>Species</TableHead>
              <TableHead>Breed</TableHead>
              <TableHead>Owner</TableHead>
              <TableHead>Veterinarian</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {patients.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} className="text-center py-8 text-muted-foreground">
                  No patients yet.
                </TableCell>
              </TableRow>
            ) : (
              patients.map((patient) => (
                <TableRow key={patient.id}>
                  <TableCell className="font-medium">{patient.id}</TableCell>
                  <TableCell>{patient.name}</TableCell>
                  <TableCell>{patient.species}</TableCell>
                  <TableCell>{patient.breed ?? "-"}</TableCell>
                  <TableCell>{patient.ownerFullName}</TableCell>
                  <TableCell>{patient.veterinarianFullName}</TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>
    </>
  )
}

export default PatientsListPage;
