import type {
  CreatePatientRequest,
  PatientResponse,
  PatientSummaryResponse,
} from "@/types";
import api from "./axios";

export async function getPatients(
  search?: string,
): Promise<PatientSummaryResponse[]> {
  const { data } = await api.get<PatientSummaryResponse[]>("/patients", {
    params: search ? { search } : undefined,
  });
  return data;
}

export async function getPatient(id: string): Promise<PatientResponse> {
  const { data } = await api.get<PatientResponse>(`/patients/${id}`);
  return data;
}

export async function createPatient(
  request: CreatePatientRequest,
): Promise<PatientResponse> {
  const formData = new FormData();
  formData.append("fullName", request.fullName);
  formData.append("address", request.address);
  if (request.photo) {
    formData.append("photo", request.photo);
  }
  const { data } = await api.post<PatientResponse>("/patients", formData);
  return data;
}
