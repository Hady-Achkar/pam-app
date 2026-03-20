import { z } from "zod";

export const createAppointmentSchema = z.object({
  dateTime: z
    .date({ error: "Date and time are required" })
    .refine((val) => val > new Date(), "Appointment should be in the future"),
  dentistId: z.string().min(1, "Please select a dentist"),
  treatmentId: z.string().min(1, "Please select a treatment"),
});

export type CreateAppointmentFormData = z.infer<typeof createAppointmentSchema>;
