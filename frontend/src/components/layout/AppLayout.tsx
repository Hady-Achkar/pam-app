import { Outlet } from "react-router-dom";
import { Toaster } from "@/components/ui/sonner";
import { Stethoscope } from "lucide-react";

export function AppLayout() {
  return (
    <div className="min-h-screen bg-background">
      <header className="border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
        <div className="mx-auto flex h-14 max-w-5xl items-center px-4">
          <Stethoscope className="mr-2 h-5 w-5 text-primary" />
          <span className="text-lg font-semibold">Dental Clinic</span>
        </div>
      </header>
      <main className="mx-auto max-w-5xl px-4 py-6">
        <Outlet />
      </main>
      <Toaster position="top-right" richColors />
    </div>
  );
}
