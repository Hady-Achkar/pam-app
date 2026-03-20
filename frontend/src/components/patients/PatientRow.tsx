import { useNavigate } from "react-router-dom";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { TableRow, TableCell } from "@/components/ui/table";
import { getImageUrl } from "@/api/axios";
import { getInitials } from "@/lib/utils";
import type { PatientSummaryResponse } from "@/types";

interface PatientRowProps {
  patient: PatientSummaryResponse;
}

export function PatientRow({ patient }: PatientRowProps) {
  const navigate = useNavigate();

  return (
    <TableRow
      className="cursor-pointer"
      onClick={() => navigate(`/patients/${patient.id}`)}
    >
      <TableCell>
        <div className="flex items-center gap-3">
          <Avatar>
            <AvatarImage
              src={getImageUrl(patient.photoUrl)}
              alt={patient.fullName}
            />
            <AvatarFallback>{getInitials(patient.fullName)}</AvatarFallback>
          </Avatar>
          <span className="font-medium">{patient.fullName}</span>
        </div>
      </TableCell>
      <TableCell className="text-muted-foreground">
        {patient.address}
      </TableCell>
      <TableCell>
        <Badge variant="secondary">{patient.appointmentCount}</Badge>
      </TableCell>
    </TableRow>
  );
}
