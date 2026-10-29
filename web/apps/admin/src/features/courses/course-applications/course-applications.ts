import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  input,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import {
  CourseApplication,
  CourseListItem,
  HttpService,
  Icon,
  MessageResponse,
  PhonePipe,
  ToastService,
} from '@ygt/libraries';

@Component({
  imports: [DatePipe, Icon, PhonePipe, RouterLink],
  templateUrl: './course-applications.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CourseApplications implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  readonly id = input.required<string>();

  protected readonly course = signal<CourseListItem | null>(null);
  protected readonly applications = signal<CourseApplication[] | null>(null);
  protected readonly downloading = signal(false);
  /** Silinmekte olan başvurunun ID'si. */
  protected readonly deletingId = signal<string | null>(null);
  protected readonly deletingAll = signal(false);

  /** TC ve doğum tarihi artık formda sorulmuyor; sadece veri varsa sütun gösterilir. */
  protected readonly hasTcNumber = computed(() => this.applications()?.some((a) => a.tcNumber) ?? false);

  ngOnInit() {
    this.#http.get<CourseListItem[]>('/api/courses', (courses) =>
      this.course.set(courses.find((course) => course.id === this.id()) ?? null),
    );
    this.#http.get<CourseApplication[]>(`/api/courses/${this.id()}/applications`, (applications) =>
      this.applications.set(applications),
    );
  }

  protected export() {
    this.downloading.set(true);
    this.#http.download(
      `/api/courses/${this.id()}/applications/export`,
      () => this.downloading.set(false),
      () => this.downloading.set(false),
    );
  }

  /** KVKK: kişi verisinin silinmesini isterse tek başvuru silinir. */
  protected remove(application: CourseApplication) {
    if (!confirm(`${application.firstName} ${application.lastName} adlı kişinin başvurusu kalıcı olarak silinsin mi?`)) {
      return;
    }

    this.deletingId.set(application.id);
    this.#http.delete(
      `/api/courses/${this.id()}/applications/${application.id}`,
      () => {
        this.applications.update((list) => list?.filter((item) => item.id !== application.id) ?? null);
        this.deletingId.set(null);
        this.#toast.success('Başvuru silindi.');
      },
      () => this.deletingId.set(null),
    );
  }

  /** KVKK: eğitimin bütün başvuruları kalıcı olarak silinir, eğitimin kendisi kalır. */
  protected removeAll() {
    const count = this.applications()?.length ?? 0;
    if (
      !confirm(
        `Bu eğitimin ${count} başvurusu kalıcı olarak silinecek. Bu işlem geri alınamaz; gerekiyorsa önce Excel'e aktarın. Devam edilsin mi?`,
      )
    ) {
      return;
    }

    this.deletingAll.set(true);
    this.#http.delete<MessageResponse>(
      `/api/courses/${this.id()}/applications`,
      (res) => {
        this.applications.set([]);
        this.deletingAll.set(false);
        this.#toast.success(res.message);
      },
      () => this.deletingAll.set(false),
    );
  }
}
