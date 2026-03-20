import {
  Table,
  TableBody,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { AppointmentRow } from "./AppointmentRow";
import type { AppointmentResponse } from "@/types";

interface AppointmentListProps {
  appointments: AppointmentResponse[];
}

export function AppointmentList({ appointments }: AppointmentListProps) {
  if (!appointments.length) {
    return (
      <div className="rounded-lg border py-8 text-center text-muted-foreground">
        No appointments yet.
      </div>
    );
  }

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Date & Time</TableHead>
          <TableHead>Treatment</TableHead>
          <TableHead>Dentist</TableHead>
          <TableHead>Status</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {appointments.map((appointment) => (
          <AppointmentRow key={appointment.id} appointment={appointment} />
        ))}
      </TableBody>
    </Table>
  );
}
