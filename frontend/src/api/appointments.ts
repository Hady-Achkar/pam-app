import type {
  AppointmentResponse,
  CreateAppointmentRequest,
} from "@/types";
import api from "./axios";

export async function createAppointment(
  request: CreateAppointmentRequest,
): Promise<AppointmentResponse> {
  const { data } = await api.post<AppointmentResponse>(
    "/appointments",
    request,
  );
  return data;
}
