import { useState } from "react";
import { useParams, Link } from "react-router-dom";
import { ArrowLeft, Plus } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Card, CardContent } from "@/components/ui/card";
import { PatientInfo } from "@/components/patients/PatientInfo";
import { AppointmentList } from "@/components/appointments/AppointmentList";
import { CreateAppointmentDialog } from "@/components/appointments/CreateAppointmentDialog";
import { usePatient } from "@/hooks/usePatient";

function BackLink() {
  return (
    <Link
      to="/patients"
      className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
    >
      <ArrowLeft className="h-4 w-4" />
      Back to Patients
    </Link>
  );
}

export function PatientDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { patient, loading, error } = usePatient(id ?? "");
  const [dialogOpen, setDialogOpen] = useState(false);

  if (!id) {
    return (
      <div className="space-y-4">
        <BackLink />
        <div className="py-8 text-center text-muted-foreground">
          Invalid patient ID.
        </div>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-4 w-32" />
        <Card>
          <CardContent className="flex items-center gap-4">
            <Skeleton className="size-24 rounded-full" />
            <div className="space-y-2">
              <Skeleton className="h-6 w-48" />
              <Skeleton className="h-4 w-64" />
            </div>
          </CardContent>
        </Card>
        <Skeleton className="h-8 w-40" />
        <Skeleton className="h-32 w-full" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <BackLink />
        <div className="py-8 text-center text-muted-foreground">
          Failed to load patient. {error}
        </div>
      </div>
    );
  }

  if (!patient) {
    return (
      <div className="space-y-4">
        <BackLink />
        <div className="py-8 text-center text-muted-foreground">
          Patient not found.
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <BackLink />

      <PatientInfo patient={patient} />

      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <h2 className="text-lg font-semibold">Appointments</h2>
          <Badge variant="secondary">{patient.appointments.length}</Badge>
        </div>
        <Button onClick={() => setDialogOpen(true)}>
          <Plus />
          New Appointment
        </Button>
      </div>

      <AppointmentList appointments={patient.appointments} />

      <CreateAppointmentDialog
        patientId={id}
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onSuccess={() => setDialogOpen(false)}
      />
    </div>
  );
}
