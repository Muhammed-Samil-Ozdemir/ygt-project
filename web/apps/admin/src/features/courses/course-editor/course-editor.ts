import {
  ChangeDetectionStrategy,
  Component,
  inject,
  input,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CourseCommand, CourseListItem, HttpService, ToastService } from '@ygt/libraries';

/** Yeni eğitim (/courses/new) ve düzenleme (/courses/:id/edit) aynı bileşen. */
@Component({
  imports: [FormsModule, RouterLink],
  templateUrl: './course-editor.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CourseEditor implements OnInit {
  readonly #http = inject(HttpService);
  readonly #router = inject(Router);
  readonly #toast = inject(ToastService);

  /** Düzenlemede adresten gelir, yeni eğitimde boş. */
  readonly id = input<string>();

  protected readonly loaded = signal(false);
  protected readonly saving = signal(false);
  protected model: CourseCommand = { title: '', content: '' };

  ngOnInit() {
    const id = this.id();
    if (!id) {
      this.loaded.set(true);
      return;
    }

    this.#http.get<CourseListItem[]>('/api/courses', (courses) => {
      const course = courses.find((item) => item.id === id);
      if (!course) {
        this.#toast.error('Eğitim bulunamadı.');
        this.#router.navigateByUrl('/courses');
        return;
      }

      this.model = { title: course.title, content: course.content ?? '' };
      this.loaded.set(true);
    });
  }

  protected save() {
    const id = this.id();
    const done = () => {
      this.#toast.success(id ? 'Eğitim güncellendi.' : 'Eğitim oluşturuldu.');
      this.#router.navigateByUrl('/courses');
    };
    const failed = () => this.saving.set(false);

    this.saving.set(true);
    if (id) {
      this.#http.put(`/api/courses/${id}`, this.model, done, failed);
    } else {
      this.#http.post('/api/courses', this.model, done, failed);
    }
  }
}
