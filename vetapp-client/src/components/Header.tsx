import {Link} from "react-router";
import {AuthButton} from "@/components/AuthButton.tsx";
import {useAuth} from "@/context/AuthProvider.tsx";

const Header = () => {
  const { isAuthenticated } = useAuth();

  return (
      <>
        <header className="bg-slate-900 fixed w-full z-50">
          <div className="container mx-auto px-4 flex items-center justify-between">
            <Link to="/" className="text-white text-2xl font-bold my-6">
              VetApp
            </Link>
            <div className="flex items-center gap-4 text-white font-medium">
              {isAuthenticated && (
                  <Link to="/dashboard">Dashboard</Link>
              )}
              {isAuthenticated && (
                  <Link to="/patients">Patients</Link>
              )}
              <AuthButton/>
            </div>
          </div>
        </header>
      </>
  )
}

export default Header;