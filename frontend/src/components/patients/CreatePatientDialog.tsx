import { useState, useRef } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import { toast } from "sonner";
import { getApiErrorMessage } from "@/api/axios";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { createPatient } from "@/api/patients";
import {
  createPatientSchema,
  type CreatePatientFormData,
} from "@/schemas/patient";
import type { PatientResponse } from "@/types";

interface CreatePatientDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSuccess: (patient: PatientResponse) => void;
}

export function CreatePatientDialog({
  open,
  onOpenChange,
  onSuccess,
}: CreatePatientDialogProps) {
  const [preview, setPreview] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const queryClient = useQueryClient();

  const {
    register,
    handleSubmit,
    setValue,
    reset,
    formState: { errors },
  } = useForm<CreatePatientFormData>({
    resolver: zodResolver(createPatientSchema),
    defaultValues: { fullName: "", address: "" },
  });

  const { mutateAsync, isPending } = useMutation({
    mutationFn: createPatient,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["patients"] });
    },
  });

  function handleOpenChange(nextOpen: boolean) {
    if (!nextOpen) {
      reset();
      if (preview) URL.revokeObjectURL(preview);
      setPreview(null);
      if (fileInputRef.current) fileInputRef.current.value = "";
    }

    onOpenChange(nextOpen);
  }

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (file) {
      setValue("photo", file, { shouldValidate: true });
      if (preview) URL.revokeObjectURL(preview);
      setPreview(URL.createObjectURL(file));
    }
  }

  function removePhoto() {
    setValue("photo", undefined);
    if (preview) URL.revokeObjectURL(preview);
    setPreview(null);
    if (fileInputRef.current) fileInputRef.current.value = "";
  }

  async function onSubmit(data: CreatePatientFormData) {
    try {
      const patient = await mutateAsync(data);
      toast.success("Patient created");
      onSuccess(patient);
    } catch (err) {
      toast.error(getApiErrorMessage(err, "Failed to create patient"));
    }
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>New Patient</DialogTitle>
          <DialogDescription>Add a new patient record.</DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="fullName">Full Name</Label>
            <Input
              id="fullName"
              placeholder="John Doe"
              {...register("fullName")}
            />
            {errors.fullName && (
              <p className="text-sm text-destructive">
                {errors.fullName.message}
              </p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="address">Address</Label>
            <Input
              id="address"
              placeholder="123 Main St"
              {...register("address")}
            />
            {errors.address && (
              <p className="text-sm text-destructive">
                {errors.address.message}
              </p>
            )}
          </div>
          <div className="space-y-2">
            <Label>Photo</Label>
            {preview ? (
              <div className="flex items-center gap-3">
                <img
                  src={preview}
                  alt="Preview"
                  className="h-16 w-16 rounded-full object-cover"
                />
                <div className="flex gap-2">
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    onClick={() => fileInputRef.current?.click()}
                  >
                    Change
                  </Button>
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    onClick={removePhoto}
                  >
                    Remove
                  </Button>
                </div>
              </div>
            ) : (
              <Button
                type="button"
                variant="outline"
                onClick={() => fileInputRef.current?.click()}
              >
                Upload Photo
              </Button>
            )}
            <input
              ref={fileInputRef}
              type="file"
              accept="image/jpeg,image/png,image/webp"
              className="hidden"
              onChange={handleFileChange}
            />
            {errors.photo && (
              <p className="text-sm text-destructive">
                {errors.photo.message}
              </p>
            )}
          </div>
          <DialogFooter showCloseButton>
            <Button type="submit" disabled={isPending}>
              {isPending && <Loader2 className="animate-spin" />}
              Create Patient
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
