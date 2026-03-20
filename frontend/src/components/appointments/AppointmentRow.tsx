import { Badge } from "@/components/ui/badge";
import { TableRow, TableCell } from "@/components/ui/table";
import { formatDateTime, isPast } from "@/lib/date";
import type { AppointmentResponse } from "@/types";

interface AppointmentRowProps {
  appointment: AppointmentResponse;
}

export function AppointmentRow({ appointment }: AppointmentRowProps) {
  const past = isPast(appointment.scheduledAt, appointment.treatment.durationMinutes);

  return (
    <TableRow className={past ? "opacity-50" : undefined}>
      <TableCell>{formatDateTime(appointment.scheduledAt)}</TableCell>
      <TableCell>
        <div className="flex items-center gap-2">
          {appointment.treatment.name}
          <Badge variant="outline">
            {appointment.treatment.durationMinutes} min
          </Badge>
        </div>
      </TableCell>
      <TableCell>{appointment.dentist.name}</TableCell>
      <TableCell>
        <Badge variant={past ? "secondary" : "default"}>
          {past ? "Past" : "Upcoming"}
        </Badge>
      </TableCell>
    </TableRow>
  );
}
