const dateTimeFormatter = new Intl.DateTimeFormat("en-US", {
  dateStyle: "medium",
  timeStyle: "short",
});

export function formatDateTime(date: string): string {
  return dateTimeFormatter.format(new Date(date));
}

export function isPast(date: string): boolean {
  return new Date(date) < new Date();
}
