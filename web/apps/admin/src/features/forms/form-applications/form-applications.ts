import {
  ChangeDetectionStrategy,
  Component,
  inject,
  input,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import {
  FormApplication,
  FormListItem,
  HackathonApplication,
  HttpService,
  PhonePipe,
} from '@ygt/libraries';

@Component({
  imports: [DatePipe, PhonePipe, RouterLink],
  templateUrl: './form-applications.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class FormApplications implements OnInit {
  readonly #http = inject(HttpService);

  readonly id = input.required<string>();

  protected readonly form = signal<FormListItem | null>(null);
  protected readonly applications = signal<FormApplication[] | null>(null);
  protected readonly teams = signal<HackathonApplication[] | null>(null);
  protected readonly downloading = signal(false);

  ngOnInit() {
    // Formun türüne göre bireysel ya da takım başvuruları yüklenir.
    this.#http.get<FormListItem[]>('/api/forms', (forms) => {
      const form = forms.find((item) => item.id === this.id()) ?? null;
      this.form.set(form);

      if (form?.type === 'hackathon') {
        this.#http.get<HackathonApplication[]>(`/api/forms/${this.id()}/hackathon-applications`, (teams) =>
          this.teams.set(teams),
        );
      } else {
        this.#http.get<FormApplication[]>(`/api/forms/${this.id()}/applications`, (applications) =>
          this.applications.set(applications),
        );
      }
    });
  }

  protected export() {
    this.downloading.set(true);
    this.#http.download(
      `/api/forms/${this.id()}/applications/export`,
      () => this.downloading.set(false),
      () => this.downloading.set(false),
    );
  }
}
