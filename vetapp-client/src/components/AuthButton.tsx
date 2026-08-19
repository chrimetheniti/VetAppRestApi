import {useAuth} from "@/context/AuthProvider.tsx";
import {Button} from "@/components/ui/button.tsx";
import {useNavigate} from "react-router";
import {toast} from "sonner";

export function AuthButton() {
    const {isAuthenticated, user, logoutUser} = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logoutUser();
        toast.success("Logged out!");
        navigate("/login");
    }

    // No public pages in this app — nothing to show when logged out.
    if (!isAuthenticated) return null;

    return (
        <div className="flex items-center gap-4">
            <span className="text-gray-700">Hi, {user?.username}</span>
            <Button variant="outline" size="sm" onClick={handleLogout}>Logout</Button>
        </div>
    )
}