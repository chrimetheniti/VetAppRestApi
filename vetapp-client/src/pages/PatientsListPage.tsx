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
import {deletePatient, getPatients} from "@/api/patients.ts";
import type {Patient} from "@/schemas/patients.ts";
import {toast} from "sonner";
import {Button} from "@/components/ui/button.tsx";
import {Pencil, Trash2} from "lucide-react";
import {useNavigate} from "react-router";
import {useAuth} from "@/context/AuthProvider.tsx";

const PatientsListPage = () => {
  const [patients, setPatients] = useState<Patient[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const {user} = useAuth();

  // Only ADMIN and RECEPTIONIST can create/edit/delete patients
  // (matches backend policies INSERT_PATIENT, EDIT_PATIENT, DELETE_PATIENT).
  const canManage = user?.role === "ADMIN" || user?.role === "RECEPTIONIST";

  const handleDelete = async (id: number) => {
    if (!window.confirm("Are you sure you want to delete this patient?")) return;
    try {
      await deletePatient(id);
      setPatients((prev) => prev.filter((p) => p.id !== id));
      toast.success("Patient has been deleted");
    } catch {
      toast.error("Error deleting patient");
    }
  };

  useEffect(() => {
    getPatients()
        .then((result) => setPatients(result.data))
        .catch(() => toast.error("Failed to load patients"))
        .finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="p-8 text-center">Loading...</div>;

  return (
      <>
        <div className="p-8">
          <div className="flex justify-between items-center mb-8">
            <h1 className="text-2xl font-bold">Patients</h1>
            {canManage && (
                <Button onClick={() => navigate("/patients/new")}>+ New Patient</Button>
            )}
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
                {canManage && <TableHead className="text-right">Actions</TableHead>}
              </TableRow>
            </TableHeader>
            <TableBody>
              {patients.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={canManage ? 7 : 6} className="text-center py-8 text-muted-foreground">
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
                        {canManage && (
                            <TableCell className="text-right space-x-2">
                              <Button
                                  variant="outline"
                                  size="icon"
                                  aria-label="Edit"
                                  onClick={() => navigate(`/patients/${patient.id}`)}
                              >
                                <Pencil className="w-4 h-4"/>
                              </Button>
                              <Button
                                  variant="outline"
                                  size="icon"
                                  aria-label="Delete"
                                  onClick={() => handleDelete(patient.id)}
                              >
                                <Trash2 className="w-4 h-4"/>
                              </Button>
                            </TableCell>
                        )}
                      </TableRow>
                  ))
              )}
            </TableBody>
          </Table>
        </div>
      </>
  );
};

export default PatientsListPage;