import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CourseListItem, HttpService, Icon, ToastService } from '@ygt/libraries';

@Component({
  imports: [DatePipe, Icon, RouterLink],
  templateUrl: './courses.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Courses implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  protected readonly courses = signal<CourseListItem[] | null>(null);

  ngOnInit() {
    this.#load();
  }

  protected setActive(course: CourseListItem, isActive: boolean) {
    this.#http.post(`/api/courses/${course.id}/${isActive ? 'reopen' : 'close'}`, null, () => {
      this.#toast.success(isActive ? 'Eğitim başvurulara açıldı.' : 'Eğitim başvurulara kapatıldı.');
      this.#load();
    });
  }

  #load() {
    this.#http.get<CourseListItem[]>('/api/courses', (courses) => this.courses.set(courses));
  }
}
