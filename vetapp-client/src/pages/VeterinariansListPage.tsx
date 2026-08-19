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
import {deleteVeterinarian, getVeterinarians} from "@/api/veterinarians.ts";
import type {Veterinarian} from "@/schemas/veterinarians.ts";
import {toast} from "sonner";
import {Button} from "@/components/ui/button.tsx";
import {Pencil, Trash2} from "lucide-react";
import {Navigate, useNavigate} from "react-router";
import {useAuth} from "@/context/AuthProvider.tsx";
import Pagination from "@/components/Pagination.tsx";

const PAGE_SIZE = 10;

const VeterinariansListPage = () => {
  const [vets, setVets] = useState<Veterinarian[]>([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [totalRecords, setTotalRecords] = useState(0);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const {user} = useAuth();

  // Only ADMIN and RECEPTIONIST see this page (matches backend VIEW_VETERINARIANS policy).
  const canManage = user?.role === "ADMIN" || user?.role === "RECEPTIONIST";

  const totalPages = Math.max(1, Math.ceil(totalRecords / PAGE_SIZE));

  useEffect(() => {
    if (!canManage) return;

    let canceled = false;

    getVeterinarians(pageNumber, PAGE_SIZE)
      .then((result) => {
        if (canceled) return;
        setVets(result.data);
        setTotalRecords(result.totalRecords);
      })
      .catch(() => {
        if (!canceled) toast.error("Failed to load veterinarians");
      })
      .finally(() => {
        if (!canceled) setLoading(false);
      });

    return () => {
      canceled = true;
    };
  }, [pageNumber, canManage]);

  // Vets viewing this route go to their own profile instead.
  if (user?.role === "VETERINARIAN") return <Navigate to="/my-profile" replace/>;
  // Owners don't belong here.
  if (!canManage) return <Navigate to="/dashboard" replace/>;

  const handleDelete = async (id: number) => {
    if (!window.confirm("Are you sure you want to delete this veterinarian?")) return;
    try {
      await deleteVeterinarian(id);
      toast.success("Veterinarian has been deleted");

      if (vets.length === 1 && pageNumber > 1) {
        setPageNumber(pageNumber - 1);
      } else {
        const result = await getVeterinarians(pageNumber, PAGE_SIZE);
        setVets(result.data);
        setTotalRecords(result.totalRecords);
      }
    } catch {
      toast.error("Error deleting veterinarian");
    }
  };

  if (loading) return <div className="p-8 text-center">Loading...</div>;

  return (
    <div className="p-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-2xl font-bold">Veterinarians</h1>
        <Button onClick={() => navigate("/veterinarians/new")}>+ New Veterinarian</Button>
      </div>
      <Table>
        <TableCaption>A list of all veterinarians.</TableCaption>
        <TableHeader className="bg-gray-100 font-bold">
          <TableRow>
            <TableHead>#</TableHead>
            <TableHead>Username</TableHead>
            <TableHead>Name</TableHead>
            <TableHead>Email</TableHead>
            <TableHead>Clinic</TableHead>
            <TableHead>Phone</TableHead>
            <TableHead className="text-right">Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {vets.length === 0 ? (
            <TableRow>
              <TableCell colSpan={7} className="text-center py-8 text-muted-foreground">
                No veterinarians yet.
              </TableCell>
            </TableRow>
          ) : (
            vets.map((vet) => (
              <TableRow key={vet.id}>
                <TableCell className="font-medium">{vet.id}</TableCell>
                <TableCell>{vet.username}</TableCell>
                <TableCell>{vet.firstname} {vet.lastname}</TableCell>
                <TableCell>{vet.email}</TableCell>
                <TableCell>{vet.clinic}</TableCell>
                <TableCell>{vet.phoneNumber ?? "-"}</TableCell>
                <TableCell className="text-right space-x-2">
                  <Button
                    variant="outline"
                    size="icon"
                    aria-label="Edit"
                    onClick={() => navigate(`/veterinarians/${vet.id}`)}
                  >
                    <Pencil className="w-4 h-4"/>
                  </Button>
                  <Button
                    variant="outline"
                    size="icon"
                    aria-label="Delete"
                    onClick={() => handleDelete(vet.id)}
                  >
                    <Trash2 className="w-4 h-4"/>
                  </Button>
                </TableCell>
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

export default VeterinariansListPage;
