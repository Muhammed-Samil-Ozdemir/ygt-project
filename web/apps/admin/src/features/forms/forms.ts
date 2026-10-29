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
import { FormListItem, HttpService, Icon, ToastService } from '@ygt/libraries';

@Component({
  imports: [DatePipe, Icon, RouterLink],
  templateUrl: './forms.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Forms implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  protected readonly forms = signal<FormListItem[] | null>(null);

  ngOnInit() {
    this.#load();
  }

  protected close(form: FormListItem) {
    this.#http.post(`/api/forms/${form.id}/close`, null, () => {
      this.#toast.success('Form başvurulara kapatıldı.');
      this.#load();
    });
  }

  /** Aynı anda tek form açık olabilir; açık olan başka form varsa kapanacağı sorulur. */
  protected reopen(form: FormListItem) {
    const openForm = this.forms()?.find((item) => item.isActive);
    if (openForm && !confirm(`"${openForm.title}" formu kapanacak. Devam edilsin mi?`)) {
      return;
    }

    this.#http.post(`/api/forms/${form.id}/reopen`, null, () => {
      this.#toast.success('Form başvurulara açıldı.');
      this.#load();
    });
  }

  #load() {
    this.#http.get<FormListItem[]>('/api/forms', (forms) => this.forms.set(forms));
  }
}
