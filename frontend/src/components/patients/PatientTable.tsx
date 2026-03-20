import {
  Table,
  TableBody,
  TableHead,
  TableHeader,
  TableRow,
  TableCell,
} from "@/components/ui/table";
import { Skeleton } from "@/components/ui/skeleton";
import { PatientRow } from "./PatientRow";
import type { PatientSummaryResponse } from "@/types";

interface PatientTableProps {
  patients?: PatientSummaryResponse[];
  loading: boolean;
  search: string;
}

export function PatientTable({ patients, loading, search }: PatientTableProps) {
  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Patient</TableHead>
          <TableHead>Address</TableHead>
          <TableHead>Appointments</TableHead>
        </TableRow>
      </TableHeader>
      {loading ? (
        <TableBody>
          {Array.from({ length: 5 }).map((_, i) => (
            <TableRow key={i}>
              <TableCell>
                <div className="flex items-center gap-3">
                  <Skeleton className="h-8 w-8 rounded-full" />
                  <Skeleton className="h-4 w-32" />
                </div>
              </TableCell>
              <TableCell>
                <Skeleton className="h-4 w-48" />
              </TableCell>
              <TableCell>
                <Skeleton className="h-5 w-8" />
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      ) : !patients?.length ? (
        <TableBody>
          <TableRow>
            <TableCell
              colSpan={3}
              className="h-32 text-center text-muted-foreground"
            >
              {search
                ? `No patients found for "${search}"`
                : "No patients yet. Add your first patient to get started."}
            </TableCell>
          </TableRow>
        </TableBody>
      ) : (
        <TableBody>
          {patients.map((patient) => (
            <PatientRow key={patient.id} patient={patient} />
          ))}
        </TableBody>
      )}
    </Table>
  );
}
