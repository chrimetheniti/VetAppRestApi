import {Navigate, Route, Routes} from "react-router";
import {Toaster} from "sonner";
import LoginPage from "@/pages/LoginPage.tsx";
import RegisterOwnerPage from "@/pages/RegisterOwnerPage.tsx";
import DashboardPage from "@/pages/DashboardPage.tsx";
import PatientsListPage from "@/pages/PatientsListPage.tsx";
import PatientPage from "@/pages/PatientPage.tsx";
import OwnersListPage from "@/pages/OwnersListPage.tsx";
import OwnerCreatePage from "@/pages/OwnerCreatePage.tsx";
import OwnerEditPage from "@/pages/OwnerEditPage.tsx";
import VeterinariansListPage from "@/pages/VeterinariansListPage.tsx";
import VeterinarianCreatePage from "@/pages/VeterinarianCreatePage.tsx";
import VeterinarianEditPage from "@/pages/VeterinarianEditPage.tsx";
import MyProfilePage from "@/pages/MyProfilePage.tsx";
import ProtectedRoute from "@/components/ProtectedRoute.tsx";
import RouterLayout from "@/components/RouterLayout.tsx";

function App() {
    return (
        <>
            <Routes>
                <Route element={<RouterLayout/>}>
                    <Route path="/login" element={<LoginPage/>}/>
                    <Route path="/register" element={<RegisterOwnerPage/>}/>
                    <Route element={<ProtectedRoute/>}>
                        <Route path="/dashboard" element={<DashboardPage/>}/>
                        <Route path="/patients" element={<PatientsListPage/>}/>
                        <Route path="/patients/new" element={<PatientPage/>}/>
                        <Route path="/patients/:patientId" element={<PatientPage/>}/>
                        <Route path="/owners" element={<OwnersListPage/>}/>
                        <Route path="/owners/new" element={<OwnerCreatePage/>}/>
                        <Route path="/owners/:ownerId" element={<OwnerEditPage/>}/>
                        <Route path="/veterinarians" element={<VeterinariansListPage/>}/>
                        <Route path="/veterinarians/new" element={<VeterinarianCreatePage/>}/>
                        <Route path="/veterinarians/:vetId" element={<VeterinarianEditPage/>}/>
                        <Route path="/my-profile" element={<MyProfilePage/>}/>
                    </Route>
                    <Route path="/" element={<Navigate to="/login" replace/>}/>
                    <Route path="*" element={<Navigate to="/login" replace/>}/>
                </Route>
            </Routes>
            <Toaster position="top-center" richColors/>
        </>
    )
}

export default App