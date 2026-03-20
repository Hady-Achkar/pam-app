export interface DentistResponse {
  id: string;
  name: string;
}

export interface TreatmentResponse {
  id: string;
  name: string;
  durationMinutes: number;
}

export interface AppointmentResponse {
  id: string;
  scheduledAt: string;
  dentist: DentistResponse;
  treatment: TreatmentResponse;
}

export interface PatientResponse {
  id: string;
  fullName: string;
  address: string;
  photoUrl: string | null;
  appointments: AppointmentResponse[];
}

export interface PatientSummaryResponse {
  id: string;
  fullName: string;
  address: string;
  photoUrl: string | null;
  appointmentCount: number;
}

export interface CreatePatientRequest {
  fullName: string;
  address: string;
  photo?: File;
}

export interface CreateAppointmentRequest {
  patientId: string;
  scheduledAt: string;
  dentistId: string;
  treatmentId: string;
}
