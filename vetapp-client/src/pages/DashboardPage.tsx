import {useAuth} from "@/context/AuthProvider.tsx";

export default function DashboardPage() {
    const {user} = useAuth();

    return (
        <>
            <div className="p-4">
                <h1 className="text-2xl font-bold mb-2">
                    Welcome back, {user?.username}!
                </h1>
                <p className="text-sm text-muted-foreground mb-8">
                    You are signed in as {user?.role}.
                </p>
                <p className="text-muted-foreground">
                    Dashboard content coming soon.
                </p>
            </div>
        </>
    )
}