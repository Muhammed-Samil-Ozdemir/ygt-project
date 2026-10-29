import { DestroyRef, inject, Signal, signal } from '@angular/core';

export interface TimeLeft {
  days: number;
  hours: number;
  minutes: number;
  seconds: number;
}

export interface TimePart {
  /** İki haneli: "05". */
  value: string;
  unit: string;
}

/** Bitiş anına kalan süre. Süre dolmuşsa null. */
export function timeLeft(endsAt: string, now: number): TimeLeft | null {
  const totalSeconds = Math.floor((new Date(endsAt).getTime() - now) / 1000);
  if (totalSeconds <= 0) {
    return null;
  }

  return {
    days: Math.floor(totalSeconds / 86400),
    hours: Math.floor((totalSeconds % 86400) / 3600),
    minutes: Math.floor((totalSeconds % 3600) / 60),
    seconds: totalSeconds % 60,
  };
}

/** Kalan süreyi ekranda kutular halinde göstermek için: gün, saat, dakika, saniye. */
export function timeParts(left: TimeLeft): TimePart[] {
  const pad = (value: number) => value.toString().padStart(2, '0');

  return [
    { value: pad(left.days), unit: 'gün' },
    { value: pad(left.hours), unit: 'saat' },
    { value: pad(left.minutes), unit: 'dakika' },
    { value: pad(left.seconds), unit: 'saniye' },
  ];
}

/** Saniyede bir güncellenen "şu an". Bileşenin alan tanımında çağrılmalı; bileşen kapanınca durur. */
export function injectNow(): Signal<number> {
  const now = signal(Date.now());
  const timer = setInterval(() => now.set(Date.now()), 1000);
  inject(DestroyRef).onDestroy(() => clearInterval(timer));
  return now.asReadonly();
}
