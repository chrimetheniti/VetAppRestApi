import {Navigate, Route, Routes} from "react-router";
import {Toaster} from "sonner";
import LoginPage from "@/pages/LoginPage.tsx";
import DashboardPage from "@/pages/DashboardPage.tsx";

function App() {
    return (
        <>
            <Routes>
                <Route path="/login" element={<LoginPage/>}/>
                <Route path="/dashboard" element={<DashboardPage/>}/>
                <Route path="/" element={<Navigate to="/login" replace/>}/>
                <Route path="*" element={<Navigate to="/login" replace/>}/>
            </Routes>
            <Toaster position="top-center" richColors/>
        </>
    )
}

export default App
