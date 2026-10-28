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
import { CourseApplication, CourseListItem, HttpService, PhonePipe } from '@ygt/libraries';

@Component({
  imports: [DatePipe, PhonePipe, RouterLink],
  templateUrl: './course-applications.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CourseApplications implements OnInit {
  readonly #http = inject(HttpService);

  readonly id = input.required<string>();

  protected readonly course = signal<CourseListItem | null>(null);
  protected readonly applications = signal<CourseApplication[] | null>(null);
  protected readonly downloading = signal(false);

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
}
