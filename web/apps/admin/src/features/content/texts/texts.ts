import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpService, SiteTextField, SiteTextsCommand, ToastService } from '@ygt/libraries';

interface TextGroup {
  name: string;
  fields: SiteTextField[];
}

/**
 * Sitedeki tek parça metinler. Alanlar API'deki tanımlardan (SiteTextCatalog) çizilir;
 * yeni metin eklemek için panelde değişiklik gerekmez.
 */
@Component({
  imports: [FormsModule],
  templateUrl: './texts.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ContentTexts implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  /** null: yükleniyor. */
  protected readonly groups = signal<TextGroup[] | null>(null);
  protected readonly saving = signal(false);

  ngOnInit() {
    this.#http.get<SiteTextField[]>('/api/site/content/texts', (fields) => {
      const groups: TextGroup[] = [];
      for (const field of fields) {
        const group = groups.find((item) => item.name === field.group);
        if (group) {
          group.fields.push(field);
        } else {
          groups.push({ name: field.group, fields: [field] });
        }
      }
      this.groups.set(groups);
    });
  }

  protected save() {
    const fields = this.groups()?.flatMap((group) => group.fields) ?? [];
    const command: SiteTextsCommand = {
      values: Object.fromEntries(fields.map((field) => [field.key, field.value])),
    };

    this.saving.set(true);
    this.#http.put(
      '/api/site/content/texts',
      command,
      () => {
        this.saving.set(false);
        this.#toast.success('Metinler kaydedildi. Değişiklik site yenilenince görünür.');
      },
      () => this.saving.set(false),
    );
  }
}
