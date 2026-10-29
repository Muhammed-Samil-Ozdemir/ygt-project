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
  Icon,
  MessageResponse,
  PhonePipe,
  ToastService,
} from '@ygt/libraries';

@Component({
  imports: [DatePipe, Icon, PhonePipe, RouterLink],
  templateUrl: './form-applications.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class FormApplications implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  readonly id = input.required<string>();

  protected readonly form = signal<FormListItem | null>(null);
  protected readonly applications = signal<FormApplication[] | null>(null);
  protected readonly teams = signal<HackathonApplication[] | null>(null);
  protected readonly downloading = signal(false);
  /** Silinmekte olan başvurunun ID'si. */
  protected readonly deletingId = signal<string | null>(null);
  protected readonly deletingAll = signal(false);

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

  /** KVKK: kişi verisinin silinmesini isterse tek başvuru silinir. */
  protected remove(application: FormApplication) {
    if (!confirm(`${application.firstName} ${application.lastName} adlı kişinin başvurusu kalıcı olarak silinsin mi?`)) {
      return;
    }

    this.deletingId.set(application.id);
    this.#http.delete(
      `/api/forms/${this.id()}/applications/${application.id}`,
      () => {
        this.applications.update((list) => list?.filter((item) => item.id !== application.id) ?? null);
        this.#deleted();
      },
      () => this.deletingId.set(null),
    );
  }

  protected removeTeam(team: HackathonApplication) {
    if (!confirm(`"${team.teamName}" takımının başvurusu (bütün üyeleriyle) kalıcı olarak silinsin mi?`)) {
      return;
    }

    this.deletingId.set(team.id);
    this.#http.delete(
      `/api/forms/${this.id()}/hackathon-applications/${team.id}`,
      () => {
        this.teams.update((list) => list?.filter((item) => item.id !== team.id) ?? null);
        this.#deleted();
      },
      () => this.deletingId.set(null),
    );
  }

  /** KVKK: formun bütün başvuruları kalıcı olarak silinir, formun kendisi kalır. */
  protected removeAll() {
    const count = (this.applications() ?? this.teams())?.length ?? 0;
    if (
      !confirm(
        `Bu formun ${count} başvurusu kalıcı olarak silinecek. Bu işlem geri alınamaz; gerekiyorsa önce Excel'e aktarın. Devam edilsin mi?`,
      )
    ) {
      return;
    }

    this.deletingAll.set(true);
    this.#http.delete<MessageResponse>(
      `/api/forms/${this.id()}/applications`,
      (res) => {
        this.applications.update((list) => list && []);
        this.teams.update((list) => list && []);
        this.deletingAll.set(false);
        this.#toast.success(res.message);
      },
      () => this.deletingAll.set(false),
    );
  }

  #deleted() {
    this.deletingId.set(null);
    this.#toast.success('Başvuru silindi.');
  }
}
