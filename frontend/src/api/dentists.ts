import type { DentistResponse } from "@/types";
import api from "./axios";

export async function getDentists(): Promise<DentistResponse[]> {
  const { data } = await api.get<DentistResponse[]>("/dentists");
  return data;
}
