import { useRef } from "react";
import { useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { toast } from "sonner";
import { getApiErrorMessage } from "@/api/axios";
import { DateTimePicker } from "@/components/ui/datetime-picker";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useAppointments } from "@/hooks/useAppointments";
import { useDentists } from "@/hooks/useDentists";
import { useTreatments } from "@/hooks/useTreatments";
import {
  createAppointmentSchema,
  type CreateAppointmentFormData,
} from "@/schemas/appointment";

interface CreateAppointmentDialogProps {
  patientId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSuccess: () => void;
}

export function CreateAppointmentDialog({
  patientId,
  open,
  onOpenChange,
  onSuccess,
}: CreateAppointmentDialogProps) {
  const {
    dentists,
    loading: dentistsLoading,
    error: dentistsError,
    refetch: refetchDentists,
  } = useDentists();
  const {
    treatments,
    loading: treatmentsLoading,
    error: treatmentsError,
    refetch: refetchTreatments,
  } = useTreatments();
  const { createAppointment, creating } = useAppointments(patientId);
  const today = useRef(new Date());
  const missingReferenceData =
    !dentistsLoading &&
    !treatmentsLoading &&
    !dentistsError &&
    !treatmentsError &&
    (dentists.length === 0 || treatments.length === 0);
  const referenceDataUnavailable =
    !!dentistsError || !!treatmentsError || missingReferenceData;

  const disabled =
    creating ||
    dentistsLoading ||
    treatmentsLoading ||
    referenceDataUnavailable;

  const {
    handleSubmit,
    control,
    reset,
    formState: { errors },
  } = useForm<CreateAppointmentFormData>({
    resolver: zodResolver(createAppointmentSchema),
    defaultValues: { dateTime: undefined, dentistId: "", treatmentId: "" },
  });

  function handleOpenChange(nextOpen: boolean) {
    if (!nextOpen) reset();
    onOpenChange(nextOpen);
  }

  async function onSubmit(data: CreateAppointmentFormData) {
    try {
      await createAppointment({
        ...data,
        patientId,
        scheduledAt: data.dateTime.toISOString(),
      });
      toast.success("Appointment created");
      onSuccess();
    } catch (err) {
      toast.error(getApiErrorMessage(err, "Failed to create appointment"));
    }
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>New Appointment</DialogTitle>
          <DialogDescription>Schedule a new appointment.</DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="dateTime">Date & Time</Label>
            <Controller
              name="dateTime"
              control={control}
              render={({ field }) => (
                <DateTimePicker
                  value={field.value}
                  onChange={field.onChange}
                  granularity="minute"
                  placeholder="Pick date and time"
                  minDate={today.current}
                />
              )}
            />
            {errors.dateTime && (
              <p className="text-sm text-destructive">
                {errors.dateTime.message}
              </p>
            )}
          </div>
          <div className="space-y-2">
            <Label>Dentist</Label>
            {dentistsError && (
              <div className="flex items-center justify-between gap-3 rounded-md border border-destructive/30 bg-destructive/5 px-3 py-2 text-sm text-destructive">
                <span>Failed to load dentists. {dentistsError}</span>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={() => refetchDentists()}
                >
                  Retry
                </Button>
              </div>
            )}
            <Controller
              name="dentistId"
              control={control}
              render={({ field }) => (
                <Select
                  value={field.value || undefined}
                  onValueChange={field.onChange}
                  disabled={dentistsLoading || !!dentistsError}
                >
                  <SelectTrigger className="w-full">
                    <SelectValue placeholder="Select a dentist" />
                  </SelectTrigger>
                  <SelectContent>
                    {dentists.map((d) => (
                      <SelectItem key={d.id} value={d.id}>
                        {d.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              )}
            />
            {errors.dentistId && (
              <p className="text-sm text-destructive">
                {errors.dentistId.message}
              </p>
            )}
          </div>
          <div className="space-y-2">
            <Label>Treatment</Label>
            {treatmentsError && (
              <div className="flex items-center justify-between gap-3 rounded-md border border-destructive/30 bg-destructive/5 px-3 py-2 text-sm text-destructive">
                <span>Failed to load treatments. {treatmentsError}</span>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={() => refetchTreatments()}
                >
                  Retry
                </Button>
              </div>
            )}
            <Controller
              name="treatmentId"
              control={control}
              render={({ field }) => (
                <Select
                  value={field.value || undefined}
                  onValueChange={field.onChange}
                  disabled={treatmentsLoading || !!treatmentsError}
                >
                  <SelectTrigger className="w-full">
                    <SelectValue placeholder="Select a treatment" />
                  </SelectTrigger>
                  <SelectContent>
                    {treatments.map((t) => (
                      <SelectItem key={t.id} value={t.id}>
                        {t.name} ({t.durationMinutes} min)
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              )}
            />
            {errors.treatmentId && (
              <p className="text-sm text-destructive">
                {errors.treatmentId.message}
              </p>
            )}
          </div>
          {missingReferenceData && (
            <div className="rounded-md border border-border bg-muted/40 px-3 py-2 text-sm text-muted-foreground">
              Appointment options are unavailable right now because dentists or
              treatments could not be loaded.
            </div>
          )}
          <DialogFooter showCloseButton>
            <Button type="submit" disabled={disabled}>
              {creating && <Loader2 className="animate-spin" />}
              Create Appointment
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
