import { useQuery } from "@tanstack/react-query";
import { getTreatments } from "@/api/treatments";

export function useTreatments() {
  const {
    data: treatments = [],
    isLoading: loading,
    error,
    refetch,
  } = useQuery({
    queryKey: ["treatments"],
    queryFn: getTreatments,
    staleTime: Infinity,
  });

  return {
    treatments,
    loading,
    error: error instanceof Error ? error.message : null,
    refetch,
  };
}
