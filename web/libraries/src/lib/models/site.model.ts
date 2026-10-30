export type HomeButton = 'course' | 'form';

export interface Countdown {
  title: string;
  endsAt: string;
}

export interface SiteSettings {
  homeButton: HomeButton;
  /** Geri sayım ayarlanmamışsa null. */
  countdown: Countdown | null;
  /** Butonun gösterdiği başvuru açık mı (açık eğitim ya da açık form var mı). Kapalıysa buton gösterilmez. */
  applicationOpen: boolean;
}

export interface SiteSettingsCommand {
  homeButton: HomeButton;
  countdownTitle: string | null;
  countdownEndsAt: string | null;
}

export interface VisitCounter {
  page: string;
  count: number;
}
