import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Card, CardContent } from "@/components/ui/card";
import { getImageUrl } from "@/api/axios";
import { getInitials } from "@/lib/utils";
import type { PatientResponse } from "@/types";

interface PatientInfoProps {
  patient: PatientResponse;
}

export function PatientInfo({ patient }: PatientInfoProps) {
  return (
    <Card>
      <CardContent className="flex items-center gap-4">
        <Avatar className="size-24">
          <AvatarImage
            src={getImageUrl(patient.photoUrl)}
            alt={patient.fullName}
          />
          <AvatarFallback className="text-2xl">
            {getInitials(patient.fullName)}
          </AvatarFallback>
        </Avatar>
        <div>
          <h1 className="text-2xl font-semibold">{patient.fullName}</h1>
          <p className="text-muted-foreground">{patient.address}</p>
        </div>
      </CardContent>
    </Card>
  );
}
