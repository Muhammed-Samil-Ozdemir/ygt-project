import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpService, SiteTextField, siteImageUrl, SiteTextsCommand, ToastService } from '@ygt/libraries';
import { ImageField } from '../../../components/image-field';
import { BrandService } from '../../../services/brand';

interface TextGroup {
  name: string;
  fields: SiteTextField[];
}

/**
 * Sitedeki tek parça metinler ve görseller (logo, sekme simgesi...). Alanlar API'deki tanımlardan
 * (SiteTextCatalog) çizilir; yeni alan eklemek için panelde değişiklik gerekmez.
 * Metinler "Kaydet" ile birlikte gönderilir, görseller seçilince hemen yüklenir.
 */
@Component({
  imports: [FormsModule, ImageField],
  templateUrl: './texts.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ContentTexts implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);
  readonly #brand = inject(BrandService);

  /** null: yükleniyor. */
  protected readonly groups = signal<TextGroup[] | null>(null);
  protected readonly saving = signal(false);
  /** Görseli yüklenen ya da kaldırılan alanın anahtarı. */
  protected readonly imageBusyKey = signal<string | null>(null);
  protected readonly imageUrl = siteImageUrl;

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
      values: Object.fromEntries(
        fields.filter((field) => field.kind !== 'image').map((field) => [field.key, field.value]),
      ),
    };

    this.saving.set(true);
    this.#http.put(
      '/api/site/content/texts',
      command,
      () => {
        this.saving.set(false);
        this.#brand.load();
        this.#toast.success('Metinler kaydedildi. Değişiklik site yenilenince görünür.');
      },
      () => this.saving.set(false),
    );
  }

  protected uploadImage(field: SiteTextField, file: File) {
    this.imageBusyKey.set(field.key);
    this.#http.upload<SiteTextField>(
      `/api/site/content/texts/${field.key}/image`,
      file,
      (updated) => {
        field.value = updated.value;
        this.#imageChanged('Görsel yüklendi.');
      },
      () => this.imageBusyKey.set(null),
    );
  }

  protected removeImage(field: SiteTextField) {
    if (!confirm(`${field.label} kaldırılsın mı?`)) {
      return;
    }

    this.imageBusyKey.set(field.key);
    this.#http.delete(
      `/api/site/content/texts/${field.key}/image`,
      () => {
        field.value = '';
        this.#imageChanged('Görsel kaldırıldı.');
      },
      () => this.imageBusyKey.set(null),
    );
  }

  #imageChanged(message: string) {
    this.imageBusyKey.set(null);
    // Logo ve sekme simgesi panelde de kullanılıyor.
    this.#brand.load();
    this.#toast.success(`${message} Değişiklik site yenilenince görünür.`);
  }
}
