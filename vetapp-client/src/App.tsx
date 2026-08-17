import {Navigate, Route, Routes} from "react-router";
import {Toaster} from "sonner";
import LoginPage from "@/pages/LoginPage.tsx";
import RegisterOwnerPage from "@/pages/RegisterOwnerPage.tsx";
import DashboardPage from "@/pages/DashboardPage.tsx";
import PatientsListPage from "@/pages/PatientsListPage.tsx";
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