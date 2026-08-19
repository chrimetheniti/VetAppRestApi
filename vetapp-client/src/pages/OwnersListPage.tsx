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
import {deleteOwner, getOwners} from "@/api/owners.ts";
import type {Owner} from "@/schemas/owners.ts";
import {toast} from "sonner";
import {Button} from "@/components/ui/button.tsx";
import {Pencil, Trash2} from "lucide-react";
import {Navigate, useNavigate} from "react-router";
import {useAuth} from "@/context/AuthProvider.tsx";
import Pagination from "@/components/Pagination.tsx";

const PAGE_SIZE = 10;

const OwnersListPage = () => {
  const [owners, setOwners] = useState<Owner[]>([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [totalRecords, setTotalRecords] = useState(0);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const {user} = useAuth();

  // Only ADMIN and RECEPTIONIST see this page (matches backend VIEW_OWNERS policy).
  const canManage = user?.role === "ADMIN" || user?.role === "RECEPTIONIST";

  const totalPages = Math.max(1, Math.ceil(totalRecords / PAGE_SIZE));

  useEffect(() => {
    if (!canManage) return; // skip fetch — we'll redirect below

    let canceled = false;

    getOwners(pageNumber, PAGE_SIZE)
      .then((result) => {
        if (canceled) return;
        setOwners(result.data);
        setTotalRecords(result.totalRecords);
      })
      .catch(() => {
        if (!canceled) toast.error("Failed to load owners");
      })
      .finally(() => {
        if (!canceled) setLoading(false);
      });

    return () => {
      canceled = true;
    };
  }, [pageNumber, canManage]);

  // Owners viewing this route go to their own profile instead.
  if (user?.role === "OWNER") return <Navigate to="/my-profile" replace/>;
  // Vets don't belong here — send them home.
  if (!canManage) return <Navigate to="/dashboard" replace/>;

  const handleDelete = async (id: number) => {
    if (!window.confirm("Are you sure you want to delete this owner?")) return;
    try {
      await deleteOwner(id);
      toast.success("Owner has been deleted");

      if (owners.length === 1 && pageNumber > 1) {
        setPageNumber(pageNumber - 1);
      } else {
        const result = await getOwners(pageNumber, PAGE_SIZE);
        setOwners(result.data);
        setTotalRecords(result.totalRecords);
      }
    } catch {
      toast.error("Error deleting owner");
    }
  };

  if (loading) return <div className="p-8 text-center">Loading...</div>;

  return (
    <div className="p-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-2xl font-bold">Owners</h1>
        <Button onClick={() => navigate("/owners/new")}>+ New Owner</Button>
      </div>
      <Table>
        <TableCaption>A list of all owners.</TableCaption>
        <TableHeader className="bg-gray-100 font-bold">
          <TableRow>
            <TableHead>#</TableHead>
            <TableHead>Username</TableHead>
            <TableHead>Name</TableHead>
            <TableHead>Email</TableHead>
            <TableHead>Phone</TableHead>
            <TableHead className="text-right">Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {owners.length === 0 ? (
            <TableRow>
              <TableCell colSpan={6} className="text-center py-8 text-muted-foreground">
                No owners yet.
              </TableCell>
            </TableRow>
          ) : (
            owners.map((owner) => (
              <TableRow key={owner.id}>
                <TableCell className="font-medium">{owner.id}</TableCell>
                <TableCell>{owner.username}</TableCell>
                <TableCell>{owner.firstname} {owner.lastname}</TableCell>
                <TableCell>{owner.email}</TableCell>
                <TableCell>{owner.phoneNumber ?? "-"}</TableCell>
                <TableCell className="text-right space-x-2">
                  <Button
                    variant="outline"
                    size="icon"
                    aria-label="Edit"
                    onClick={() => navigate(`/owners/${owner.id}`)}
                  >
                    <Pencil className="w-4 h-4"/>
                  </Button>
                  <Button
                    variant="outline"
                    size="icon"
                    aria-label="Delete"
                    onClick={() => handleDelete(owner.id)}
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

export default OwnersListPage;
