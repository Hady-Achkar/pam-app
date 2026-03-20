import { useQuery } from "@tanstack/react-query";
import { getDentists } from "@/api/dentists";

export function useDentists() {
  const {
    data: dentists = [],
    isLoading: loading,
    error,
    refetch,
  } = useQuery({
    queryKey: ["dentists"],
    queryFn: getDentists,
  });

  return {
    dentists,
    loading,
    error: error instanceof Error ? error.message : null,
    refetch,
  };
}
