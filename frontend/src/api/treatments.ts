import type { TreatmentResponse } from "@/types";
import api from "./axios";

export async function getTreatments(): Promise<TreatmentResponse[]> {
  const { data } = await api.get<TreatmentResponse[]>("/treatments");
  return data;
}
