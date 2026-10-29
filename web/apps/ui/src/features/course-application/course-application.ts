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
import { Course, CourseApplicationCommand, HttpService, Icon, ToastService } from '@ygt/libraries';
import { ApplicationSent } from '../../components/application-sent';

@Component({
  imports: [ApplicationSent, FormsModule, Icon, RouterLink],
  templateUrl: './course-application.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CourseApplication implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  /** null: yükleniyor. */
  protected readonly courses = signal<Course[] | null>(null);
  protected readonly sending = signal(false);
  protected readonly sent = signal(false);

  protected courseId = '';
  protected model: CourseApplicationCommand = { firstName: '', lastName: '', email: '', phoneNumber: '' };

  ngOnInit() {
    this.#http.get<Course[]>(
      '/api/courses/active',
      (courses) => {
        // Tek eğitim varsa seçili gelsin.
        if (courses.length === 1) {
          this.courseId = courses[0].id;
        }
        this.courses.set(courses);
      },
      () => this.courses.set([]),
    );
  }

  protected contentOf(courseId: string) {
    return this.courses()?.find((course) => course.id === courseId)?.content ?? null;
  }

  protected submit() {
    if (!this.courseId) {
      this.#toast.error('Lütfen bir eğitim seçin.');
      return;
    }

    this.sending.set(true);
    this.#http.post(
      `/api/courses/${this.courseId}/applications`,
      this.model,
      () => {
        this.sending.set(false);
        this.sent.set(true);
      },
      () => this.sending.set(false),
    );
  }
}
