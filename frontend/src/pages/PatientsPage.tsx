import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { PatientTable } from "@/components/patients/PatientTable";
import { CreatePatientDialog } from "@/components/patients/CreatePatientDialog";
import { usePatients } from "@/hooks/usePatients";

export function PatientsPage() {
  const navigate = useNavigate();
  const [dialogOpen, setDialogOpen] = useState(false);
  const { patients, loading, error, search, setSearch, refetch } =
    usePatients();

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Patients</h1>
        <Button onClick={() => setDialogOpen(true)}>
          <Plus />
          New Patient
        </Button>
      </div>

      <Input
        placeholder="Search patients..."
        aria-label="Search patients"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
      />

      {error ? (
        <div className="py-8 text-center">
          <p className="text-muted-foreground">{error}</p>
          <Button variant="outline" onClick={() => refetch()} className="mt-2">
            Retry
          </Button>
        </div>
      ) : (
        <PatientTable patients={patients} loading={loading} search={search} />
      )}

      <CreatePatientDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onSuccess={(patient) => {
          setDialogOpen(false);
          navigate(`/patients/${patient.id}`);
        }}
      />
    </div>
  );
}
