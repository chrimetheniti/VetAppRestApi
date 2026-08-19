import {useEffect, useState} from "react";
import type {ReactNode} from "react";
import {useNavigate} from "react-router";
import {useAuth} from "@/context/AuthProvider.tsx";
import {getPatients} from "@/api/patients.ts";
import {getOwners} from "@/api/owners.ts";
import {getVeterinarians} from "@/api/veterinarians.ts";
import {PawPrint, Users, Stethoscope, Plus, UserCog, ArrowRight} from "lucide-react";

// ================= Card components =================

// Primary action card — filled deep teal, white text, big plus icon in circle.
// Used for the primary CTA of each role.
const PrimaryActionCard = ({label, subtitle, onClick}: {
    label: string; subtitle: string; onClick: () => void;
}) => (
    <button
        onClick={onClick}
        className="rounded-xl p-6 bg-teal-700 hover:bg-teal-800 transition text-left cursor-pointer flex items-center justify-between text-white"
    >
        <div className="flex items-center gap-4">
            <div className="w-12 h-12 rounded-full bg-teal-600 flex items-center justify-center flex-shrink-0">
                <Plus className="w-6 h-6 text-white"/>
            </div>
            <div>
                <div className="font-semibold text-lg">{label}</div>
                <div className="text-sm text-teal-100">{subtitle}</div>
            </div>
        </div>
        <ArrowRight className="w-5 h-5 text-white flex-shrink-0"/>
    </button>
);

// Secondary card — outlined, teal-tint icon circle, "N registered" style subtitle.
const OutlinedActionCard = ({label, subtitle, icon, onClick}: {
    label: string; subtitle: string; icon: ReactNode; onClick: () => void;
}) => (
    <button
        onClick={onClick}
        className="rounded-xl p-6 bg-white border hover:border-teal-600 hover:shadow-md transition text-left cursor-pointer flex items-center justify-between"
    >
        <div className="flex items-center gap-4">
            <div className="w-12 h-12 rounded-full bg-teal-100 flex items-center justify-center text-teal-700 flex-shrink-0">
                {icon}
            </div>
            <div>
                <div className="font-semibold text-lg text-gray-900">{label}</div>
                <div className="text-sm text-gray-500">{subtitle}</div>
            </div>
        </div>
        <ArrowRight className="w-5 h-5 text-gray-400 flex-shrink-0"/>
    </button>
);

// ================= Page =================

export default function DashboardPage() {
    const {user} = useAuth();
    const navigate = useNavigate();

    const isAdminOrReceptionist = user?.role === "ADMIN" || user?.role === "RECEPTIONIST";
    const isVet = user?.role === "VETERINARIAN";
    const isOwner = user?.role === "OWNER";

    const [patientCount, setPatientCount] = useState<number | null>(null);
    const [ownerCount, setOwnerCount] = useState<number | null>(null);
    const [vetCount, setVetCount] = useState<number | null>(null);

    useEffect(() => {
        let canceled = false;

        // Everyone with VIEW_PATIENTS gets the patient count.
        if (isAdminOrReceptionist || isVet) {
            getPatients(1, 1)
                .then((res) => !canceled && setPatientCount(res.totalRecords))
                .catch(() => !canceled && setPatientCount(null));
        }

        // Only admin/receptionist have VIEW_OWNERS + VIEW_VETERINARIANS.
        if (isAdminOrReceptionist) {
            getOwners(1, 1)
                .then((res) => !canceled && setOwnerCount(res.totalRecords))
                .catch(() => !canceled && setOwnerCount(null));
            getVeterinarians(1, 1)
                .then((res) => !canceled && setVetCount(res.totalRecords))
                .catch(() => !canceled && setVetCount(null));
        }

        return () => {
            canceled = true;
        };
    }, [isAdminOrReceptionist, isVet]);

    // Format count like "24 registered", "5 in clinic", or "…" while loading.
    const countLabel = (n: number | null, singular: string, plural: string) =>
        n === null ? "…" : `${n} ${n === 1 ? singular : plural}`;

    // Format today's date like "Wednesday, 19 August 2026" for the greeting subtitle.
    const today = new Date().toLocaleDateString("en-GB", {
        weekday: "long", day: "numeric", month: "long", year: "numeric",
    });

    return (
        <div className="p-8 max-w-5xl mx-auto">
            {/* Greeting */}
            <h1 className="text-3xl font-bold mb-1">
                Welcome back, {user?.username} <span className="inline-block">👋</span>
            </h1>
            <p className="text-sm text-gray-500 mb-10">
                Signed in as {user?.role} · {today}
            </p>

            <h2 className="text-xs uppercase tracking-wider font-semibold text-gray-500 mb-4">
                Quick actions
            </h2>

            {/* ===== ADMIN / RECEPTIONIST ===== */}
            {isAdminOrReceptionist && (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <PrimaryActionCard
                        label="Add new patient"
                        subtitle="Register a new animal"
                        onClick={() => navigate("/patients/new")}
                    />
                    <OutlinedActionCard
                        label="View patients"
                        subtitle={countLabel(patientCount, "registered", "registered")}
                        icon={<PawPrint className="w-6 h-6"/>}
                        onClick={() => navigate("/patients")}
                    />
                    <OutlinedActionCard
                        label="Owners"
                        subtitle={countLabel(ownerCount, "registered", "registered")}
                        icon={<Users className="w-6 h-6"/>}
                        onClick={() => navigate("/owners")}
                    />
                    <OutlinedActionCard
                        label="Veterinarians"
                        subtitle={countLabel(vetCount, "in clinic", "in clinic")}
                        icon={<Stethoscope className="w-6 h-6"/>}
                        onClick={() => navigate("/veterinarians")}
                    />
                </div>
            )}

            {/* ===== VETERINARIAN ===== */}
            {isVet && (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <OutlinedActionCard
                        label="View patients"
                        subtitle={countLabel(patientCount, "in clinic", "in clinic")}
                        icon={<PawPrint className="w-6 h-6"/>}
                        onClick={() => navigate("/patients")}
                    />
                    <OutlinedActionCard
                        label="My profile"
                        subtitle="View and update your details"
                        icon={<UserCog className="w-6 h-6"/>}
                        onClick={() => navigate("/my-profile")}
                    />
                </div>
            )}

            {/* ===== OWNER ===== */}
            {isOwner && (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <OutlinedActionCard
                        label="My profile"
                        subtitle="View and update your details"
                        icon={<UserCog className="w-6 h-6"/>}
                        onClick={() => navigate("/my-profile")}
                    />
                </div>
            )}
        </div>
    );
}