import {Link, NavLink} from "react-router";
import {AuthButton} from "@/components/AuthButton.tsx";
import {useAuth} from "@/context/AuthProvider.tsx";

const Header = () => {
    const {isAuthenticated, user} = useAuth();

    // Admin and Receptionist manage the clinic — full menu.
    const canManageClinic = user?.role === "ADMIN" || user?.role === "RECEPTIONIST";
    // Vets see patients + their own profile.
    const isVet = user?.role === "VETERINARIAN";
    // Owners see their own pets and their own profile.
    const isOwner = user?.role === "OWNER";

    // NavLink helper — teal underline when the route is active.
    const linkClass = ({isActive}: {isActive: boolean}) =>
        isActive
            ? "text-teal-700 font-semibold border-b-2 border-teal-700 pb-1"
            : "text-gray-700 hover:text-teal-700 transition pb-1";

    return (
        <header className="bg-white fixed w-full z-50 border-b border-gray-200 shadow-sm">
            <div className="container mx-auto px-4 flex items-center justify-between h-20">
                {/* Logo + name */}
                <Link to="/" className="flex items-center gap-3">
                    <img src="/logo.png" alt="VetApp" className="h-12 w-auto"/>
                    <span className="text-2xl font-bold text-gray-900">VetApp</span>
                </Link>

                {/* Nav links + auth */}
                <div className="flex items-center gap-6 font-medium">
                    {isAuthenticated && (
                        <NavLink to="/dashboard" className={linkClass}>Dashboard</NavLink>
                    )}
                    {isAuthenticated && (canManageClinic || isVet) && (
                        <NavLink to="/patients" className={linkClass}>Patients</NavLink>
                    )}
                    {isAuthenticated && isOwner && (
                        <NavLink to="/patients" className={linkClass}>My Pets</NavLink>
                    )}
                    {isAuthenticated && canManageClinic && (
                        <NavLink to="/owners" className={linkClass}>Owners</NavLink>
                    )}
                    {isAuthenticated && canManageClinic && (
                        <NavLink to="/veterinarians" className={linkClass}>Veterinarians</NavLink>
                    )}
                    {isAuthenticated && (isVet || isOwner) && (
                        <NavLink to="/my-profile" className={linkClass}>My Profile</NavLink>
                    )}
                    <AuthButton/>
                </div>
            </div>
        </header>
    )
}

export default Header;