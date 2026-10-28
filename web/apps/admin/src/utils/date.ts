/** ISO tarihi datetime-local alanının beklediği yerel biçime çevirir: "2026-12-05T17:30". */
export function toLocalInput(iso: string | null | undefined): string {
  if (!iso) {
    return '';
  }

  const date = new Date(iso);
  const pad = (value: number) => value.toString().padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}

/** datetime-local değerini (tarayıcının saat dilimiyle) UTC ISO tarihine çevirir. */
export function fromLocalInput(value: string): string | null {
  return value ? new Date(value).toISOString() : null;
}
