import {Navigate} from "react-router";
import {useAuth} from "@/context/AuthProvider.tsx";
import MyOwnerProfilePage from "@/pages/MyOwnerProfilePage.tsx";
import MyVetProfilePage from "@/pages/MyVetProfilePage.tsx";

// Dispatcher: routes to the correct self-profile screen based on user role.
// Admin/Receptionist don't have a personal profile — they manage others.
const MyProfilePage = () => {
  const {user} = useAuth();

  if (user?.role === "OWNER" && user.ownerId) {
    return <MyOwnerProfilePage ownerId={user.ownerId}/>;
  }

  if (user?.role === "VETERINARIAN" && user.veterinarianId) {
    return <MyVetProfilePage vetId={user.veterinarianId}/>;
  }

  return <Navigate to="/dashboard" replace/>;
};

export default MyProfilePage;