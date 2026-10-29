import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  CourseListItem,
  FormListItem,
  HomeButton,
  HttpService,
  Icon,
  SiteSettings,
  SiteSettingsCommand,
  ToastService,
  VisitCounter,
} from '@ygt/libraries';
import { fromLocalInput, toLocalInput } from '../../utils/date';

@Component({
  imports: [FormsModule, Icon, RouterLink],
  templateUrl: './home.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Home implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  protected readonly visits = signal<number | null>(null);
  protected readonly activeCourses = signal<number | null>(null);
  /** undefined: yükleniyor, null: açık form yok. */
  protected readonly activeForm = signal<FormListItem | null | undefined>(undefined);

  protected readonly settingsLoaded = signal(false);
  protected readonly saving = signal(false);
  protected readonly hasCountdown = signal(false);
  protected settings = { homeButton: 'form' as HomeButton, countdownTitle: '', countdownEndsAt: '' };

  ngOnInit() {
    this.#http.get<VisitCounter[]>('/api/site/visits', (counters) =>
      this.visits.set(counters.find((counter) => counter.page === 'home')?.count ?? 0),
    );
    this.#http.get<CourseListItem[]>('/api/courses', (courses) =>
      this.activeCourses.set(courses.filter((course) => course.isActive).length),
    );
    this.#http.get<FormListItem[]>('/api/forms', (forms) =>
      this.activeForm.set(forms.find((form) => form.isActive) ?? null),
    );
    this.#loadSettings();
  }

  protected saveSettings() {
    const command: SiteSettingsCommand = {
      homeButton: this.settings.homeButton,
      countdownTitle: this.settings.countdownTitle.trim() || null,
      countdownEndsAt: fromLocalInput(this.settings.countdownEndsAt),
    };

    this.saving.set(true);
    this.#http.put(
      '/api/site/settings',
      command,
      () => {
        this.saving.set(false);
        this.hasCountdown.set(command.countdownTitle !== null);
        this.#toast.success('Ayarlar kaydedildi.');
      },
      () => this.saving.set(false),
    );
  }

  protected removeCountdown() {
    this.settings.countdownTitle = '';
    this.settings.countdownEndsAt = '';
    this.saveSettings();
  }

  #loadSettings() {
    this.#http.get<SiteSettings>('/api/site/settings', (settings) => {
      this.settings = {
        homeButton: settings.homeButton,
        countdownTitle: settings.countdown?.title ?? '',
        countdownEndsAt: toLocalInput(settings.countdown?.endsAt),
      };
      this.hasCountdown.set(settings.countdown !== null);
      this.settingsLoaded.set(true);
    });
  }
}
