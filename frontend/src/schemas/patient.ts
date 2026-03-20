import { z } from "zod";

const MAX_FILE_SIZE = 5 * 1024 * 1024;
const ACCEPTED_IMAGE_TYPES = ["image/jpeg", "image/png", "image/webp"];

export const createPatientSchema = z.object({
  fullName: z
    .string()
    .min(2, "Name should be at least 2 characters")
    .max(100, "Name should be under 100 characters"),
  address: z
    .string()
    .min(1, "Address is required")
    .max(250, "Address should be under 250 characters"),
  photo: z
    .instanceof(File)
    .refine((file) => file.size <= MAX_FILE_SIZE, "Photo should be under 5MB")
    .refine(
      (file) => ACCEPTED_IMAGE_TYPES.includes(file.type),
      "Only JPEG, PNG, and WebP images are allowed",
    )
    .optional(),
});

export type CreatePatientFormData = z.infer<typeof createPatientSchema>;
