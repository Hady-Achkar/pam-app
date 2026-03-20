import { useQuery } from "@tanstack/react-query";
import { useSearchParams } from "react-router-dom";
import { getPatients } from "@/api/patients";
import { useDebounce } from "./useDebounce";

export function usePatients() {
  const [searchParams, setSearchParams] = useSearchParams();
  const search = searchParams.get("search") ?? "";
  const debouncedSearch = useDebounce(search);

  const {
    data: patients,
    isLoading: loading,
    error,
    refetch,
  } = useQuery({
    queryKey: ["patients", debouncedSearch],
    queryFn: () => getPatients(debouncedSearch || undefined),
  });

  function setSearch(value: string) {
    setSearchParams((prev) => {
      const next = new URLSearchParams(prev);

      if (value) {
        next.set("search", value);
      } else {
        next.delete("search");
      }

      return next;
    });
  }

  return {
    patients,
    loading,
    error: error instanceof Error ? error.message : null,
    search,
    setSearch,
    refetch,
  };
}
