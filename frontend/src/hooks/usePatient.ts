import { useQuery } from "@tanstack/react-query";
import { getPatient } from "@/api/patients";

export function usePatient(id: string) {
  const {
    data: patient,
    isLoading: loading,
    error,
    refetch,
  } = useQuery({
    queryKey: ["patient", id],
    queryFn: () => getPatient(id),
    enabled: !!id,
  });

  return {
    patient,
    loading,
    error: error instanceof Error ? error.message : null,
    refetch,
  };
}
