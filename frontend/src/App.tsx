import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ErrorBoundary } from "@/components/ErrorBoundary";
import { AppLayout } from "@/components/layout/AppLayout";
import { PatientsPage } from "@/pages/PatientsPage";
import { PatientDetailPage } from "@/pages/PatientDetailPage";

// don't retry indefinitely
// assume data is fresh for 30 seconds
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      staleTime: 30 * 1000,
      refetchOnWindowFocus: false,
    },
  },
});

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ErrorBoundary>
        {/* routes can be defined in a separate file for better organization */}
        <BrowserRouter>
          <Routes>
            <Route element={<AppLayout />}>
              <Route path="/patients" element={<PatientsPage />} />
              <Route path="/patients/:id" element={<PatientDetailPage />} />
              <Route path="/" element={<Navigate to="/patients" replace />} />
              <Route path="*" element={<Navigate to="/patients" replace />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </ErrorBoundary>
    </QueryClientProvider>
  );
}
