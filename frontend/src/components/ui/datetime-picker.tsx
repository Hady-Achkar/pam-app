import * as React from "react";
import { format } from "date-fns";
import { Calendar as CalendarIcon, Clock } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Input } from "@/components/ui/input";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { cn } from "@/lib/utils";

type Granularity = "day" | "minute";

interface DateTimePickerProps {
  value?: Date;
  onChange?: (date: Date | undefined) => void;
  disabled?: boolean;
  placeholder?: string;
  granularity?: Granularity;
  minDate?: Date;
  className?: string;
}

function DateTimePicker({
  value,
  onChange,
  disabled = false,
  placeholder = "Pick a date",
  granularity = "day",
  minDate,
  className,
}: DateTimePickerProps) {
  const [open, setOpen] = React.useState(false);

  const hours = value ? String(value.getHours()).padStart(2, "0") : "";
  const minutes = value ? String(value.getMinutes()).padStart(2, "0") : "";

  function handleDateSelect(day: Date | undefined) {
    if (!day) return;
    const next = new Date(day);
    if (value) {
      next.setHours(value.getHours(), value.getMinutes(), 0, 0);
    }
    onChange?.(next);
  }

  function handleTimeChange(type: "hours" | "minutes", raw: string) {
    const num = parseInt(raw, 10);
    if (Number.isNaN(num)) return;

    const max = type === "hours" ? 23 : 59;
    const clamped = Math.max(0, Math.min(num, max));

    const next = value ? new Date(value) : new Date();
    if (!value) next.setSeconds(0, 0);

    if (type === "hours") next.setHours(clamped);
    else next.setMinutes(clamped);

    onChange?.(next);
  }

  const displayValue = value
    ? granularity === "minute"
      ? format(value, "d MMM yyyy, HH:mm")
      : format(value, "d MMM yyyy")
    : undefined;

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild disabled={disabled}>
        <Button
          variant="outline"
          className={cn(
            "w-full justify-start text-left font-normal",
            !value && "text-muted-foreground",
            className,
          )}
        >
          <CalendarIcon className="mr-2 h-4 w-4" />
          {displayValue ?? <span>{placeholder}</span>}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0" align="start">
        <Calendar
          mode="single"
          selected={value}
          onSelect={handleDateSelect}
          disabled={minDate ? { before: minDate } : undefined}
          defaultMonth={value ?? minDate}
        />
        {granularity === "minute" && (
          <div className="flex items-center gap-2 border-t px-3 py-3">
            <Clock className="h-4 w-4 text-muted-foreground" />
            <Input
              type="number"
              min={0}
              max={23}
              placeholder="hr."
              value={hours}
              onChange={(e) => handleTimeChange("hours", e.target.value)}
              className="w-20 text-center tabular-nums [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
            />
            <span className="text-muted-foreground">:</span>
            <Input
              type="number"
              min={0}
              max={59}
              placeholder="min."
              value={minutes}
              onChange={(e) => handleTimeChange("minutes", e.target.value)}
              className="w-20 text-center tabular-nums [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
            />
          </div>
        )}
      </PopoverContent>
    </Popover>
  );
}

export { DateTimePicker };
export type { DateTimePickerProps };
