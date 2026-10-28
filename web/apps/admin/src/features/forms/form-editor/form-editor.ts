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
import { FormCommand, FormListItem, HttpService, ToastService } from '@ygt/libraries';

/** Yeni form (/forms/new) ve düzenleme (/forms/:id/edit) aynı bileşen. */
@Component({
  imports: [FormsModule, RouterLink],
  templateUrl: './form-editor.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class FormEditor implements OnInit {
  readonly #http = inject(HttpService);
  readonly #router = inject(Router);
  readonly #toast = inject(ToastService);

  readonly id = input<string>();

  protected readonly loaded = signal(false);
  protected readonly saving = signal(false);
  protected readonly hasApplications = signal(false);
  protected model: FormCommand = { title: '', content: '', type: 'standard' };

  ngOnInit() {
    const id = this.id();
    if (!id) {
      this.loaded.set(true);
      return;
    }

    this.#http.get<FormListItem[]>('/api/forms', (forms) => {
      const form = forms.find((item) => item.id === id);
      if (!form) {
        this.#toast.error('Form bulunamadı.');
        this.#router.navigateByUrl('/forms');
        return;
      }

      this.model = { title: form.title, content: form.content ?? '', type: form.type };
      this.hasApplications.set(form.applicationCount > 0);
      this.loaded.set(true);
    });
  }

  protected save() {
    const id = this.id();
    const done = () => {
      this.#toast.success(id ? 'Form güncellendi.' : 'Form oluşturuldu ve başvurulara açıldı.');
      this.#router.navigateByUrl('/forms');
    };
    const failed = () => this.saving.set(false);

    this.saving.set(true);
    if (id) {
      this.#http.put(`/api/forms/${id}`, this.model, done, failed);
    } else {
      this.#http.post('/api/forms', this.model, done, failed);
    }
  }
}
