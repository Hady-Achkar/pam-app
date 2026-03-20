const dateTimeFormatter = new Intl.DateTimeFormat("en-US", {
  dateStyle: "medium",
  timeStyle: "short",
});

export function formatDateTime(date: string): string {
  return dateTimeFormatter.format(new Date(date));
}

export function isPast(date: string, durationMinutes: number): boolean {
  return new Date(date).getTime() + durationMinutes * 60_000 < Date.now();
}
