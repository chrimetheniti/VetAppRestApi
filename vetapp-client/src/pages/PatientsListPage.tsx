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
import Pagination from "@/components/Pagination.tsx";

const PAGE_SIZE = 10;

const PatientsListPage = () => {
  const [patients, setPatients] = useState<Patient[]>([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [totalRecords, setTotalRecords] = useState(0);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const {user} = useAuth();

  // Permission model (server-side rules mirror this):
  // - canCreate / canDelete: ADMIN and RECEPTIONIST only (clinic staff).
  // - canEdit: ADMIN, RECEPTIONIST, and OWNER (owners can edit their own pets;
  //   the backend blocks reassignment attempts server-side).
  // - VETERINARIAN sees the list read-only (no actions column).
  const canCreate = user?.role === "ADMIN" || user?.role === "RECEPTIONIST";
  const canDelete = canCreate;
  const canEdit = canCreate || user?.role === "OWNER";
  const showActions = canEdit || canDelete;

  const totalPages = Math.max(1, Math.ceil(totalRecords / PAGE_SIZE));

  // Fetch the current page whenever pageNumber changes.
  useEffect(() => {
    let canceled = false;

    getPatients(pageNumber, PAGE_SIZE)
        .then((result) => {
          if (canceled) return;
          setPatients(result.data);
          setTotalRecords(result.totalRecords);
        })
        .catch(() => {
          if (!canceled) toast.error("Failed to load patients");
        })
        .finally(() => {
          if (!canceled) setLoading(false);
        });

    return () => {
      canceled = true;
    };
  }, [pageNumber]);

  const handleDelete = async (id: number) => {
    if (!window.confirm("Are you sure you want to delete this patient?")) return;
    try {
      await deletePatient(id);
      toast.success("Patient has been deleted");

      if (patients.length === 1 && pageNumber > 1) {
        setPageNumber(pageNumber - 1);
      } else {
        const result = await getPatients(pageNumber, PAGE_SIZE);
        setPatients(result.data);
        setTotalRecords(result.totalRecords);
      }
    } catch {
      toast.error("Error deleting patient");
    }
  };

  if (loading) return <div className="p-8 text-center">Loading...</div>;

  // Title changes per role — owners see "My Pets" here, everyone else sees "Patients".
  const pageTitle = user?.role === "OWNER" ? "My Pets" : "Patients";

  return (
      <div className="p-8">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-2xl font-bold">{pageTitle}</h1>
          {canCreate && (
              <Button onClick={() => navigate("/patients/new")}>+ New Patient</Button>
          )}
        </div>
        <Table>
          <TableCaption>
            {user?.role === "OWNER" ? "Your pets registered at the clinic." : "A list of all patients."}
          </TableCaption>
          <TableHeader className="bg-gray-100 font-bold">
            <TableRow>
              <TableHead>#</TableHead>
              <TableHead>Name</TableHead>
              <TableHead>Species</TableHead>
              <TableHead>Breed</TableHead>
              <TableHead>Owner</TableHead>
              <TableHead>Veterinarian</TableHead>
              {showActions && <TableHead className="text-right">Actions</TableHead>}
            </TableRow>
          </TableHeader>
          <TableBody>
            {patients.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={showActions ? 7 : 6} className="text-center py-8 text-muted-foreground">
                    {user?.role === "OWNER" ? "You have no pets registered yet." : "No patients yet."}
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
                      {showActions && (
                          <TableCell className="text-right space-x-2">
                            {canEdit && (
                                <Button
                                    variant="outline"
                                    size="icon"
                                    aria-label="Edit"
                                    onClick={() => navigate(`/patients/${patient.id}`)}
                                >
                                  <Pencil className="w-4 h-4"/>
                                </Button>
                            )}
                            {canDelete && (
                                <Button
                                    variant="outline"
                                    size="icon"
                                    aria-label="Delete"
                                    onClick={() => handleDelete(patient.id)}
                                >
                                  <Trash2 className="w-4 h-4"/>
                                </Button>
                            )}
                          </TableCell>
                      )}
                    </TableRow>
                ))
            )}
          </TableBody>
        </Table>

        <Pagination
            pageNumber={pageNumber}
            totalPages={totalPages}
            onPageChange={setPageNumber}
        />
      </div>
  );
};

export default PatientsListPage;