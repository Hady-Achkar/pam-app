import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createAppointment } from "@/api/appointments";
import type { CreateAppointmentRequest } from "@/types";

export function useAppointments(patientId: string) {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (data: CreateAppointmentRequest) => createAppointment(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["patient", patientId] });
      queryClient.invalidateQueries({ queryKey: ["patients"] });
    },
  });

  return {
    createAppointment: mutation.mutateAsync,
    creating: mutation.isPending,
  };
}
