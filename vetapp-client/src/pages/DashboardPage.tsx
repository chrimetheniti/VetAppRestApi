import {useNavigate} from "react-router";
import {useAuth} from "@/context/AuthProvider.tsx";
import {Button} from "@/components/ui/button.tsx";
import {toast} from "sonner";

export default function DashboardPage() {
    const {user, logoutUser} = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logoutUser();
        toast.success("Logged out");
        navigate("/login");
    }

    return (
        <>
            <div className="min-h-screen p-8">
                <div className="max-w-4xl mx-auto">
                    <div className="flex items-center justify-between mb-8">
                        <div>
                            <h1 className="text-2xl font-bold">
                                Welcome back, {user?.username}!
                            </h1>
                            <p className="text-sm text-muted-foreground">
                                You are signed in as {user?.role}.
                            </p>
                        </div>
                        <Button variant="outline" onClick={handleLogout}>
                            Log out
                        </Button>
                    </div>
                    <p className="text-muted-foreground">
                        Dashboard content coming soon.
                    </p>
                </div>
            </div>
        </>
    )
}
