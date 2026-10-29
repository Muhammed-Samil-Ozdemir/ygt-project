import { ChangeDetectionStrategy, Component, inject, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  HttpService,
  Icon,
  OrderCommand,
  SiteEventCommand,
  SiteEventListItem,
  SiteEventPhoto,
  SiteEventStyle,
  siteImageUrl,
  ToastService,
} from '@ygt/libraries';
import { EditableList } from '../../../utils/editable-list';

/** API'deki sınırla aynı. */
const MAX_PHOTOS = 12;

const STYLES: { value: SiteEventStyle; label: string; hint: string }[] = [
  {
    value: 'wall',
    label: 'Etkinlik panosu',
    hint: 'Panoda polaroid olarak görünür: kapak fotoğrafı ve başlık. Açıklama sadece fotoğraflar açılınca görünür.',
  },
  {
    value: 'featured',
    label: 'Öne çıkan',
    hint: 'Kendi bloğunda büyük başlık, açıklama ve ilk üç fotoğraf kolaj olarak görünür.',
  },
  {
    value: 'recurring',
    label: 'Geleneksel',
    hint: '"Gelenekselleşmiş etkinliklerimiz" bölümünde kapak fotoğrafı ve yanında el yazısıyla açıklama olarak görünür.',
  },
];

const emptyEvent = (): SiteEventCommand => ({ title: '', description: '', style: 'wall', isVisible: true });

/**
 * Ana sayfadaki etkinlikler. Metinler "Kaydet" ile gönderilir; fotoğraflar seçilince hemen yüklenir,
 * silinir ya da sıralanır. İlk fotoğraf kapaktır.
 */
@Component({
  imports: [FormsModule, Icon],
  templateUrl: './events.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ContentEvents extends EditableList<SiteEventListItem, SiteEventCommand> {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  protected readonly endpoint = '/api/site/content/events';
  protected readonly imageUrl = siteImageUrl;
  protected readonly styles = STYLES;
  protected readonly maxPhotos = MAX_PHOTOS;
  protected newEvent = emptyEvent();

  protected toCommand(event: SiteEventListItem): SiteEventCommand {
    // Boş açıklama null gider; böylece silinip boş bırakılan açıklama "kaydedilmedi" sayılmaz.
    return { title: event.title, description: event.description || null, style: event.style, isVisible: event.isVisible };
  }

  protected styleHint(style: SiteEventStyle) {
    return STYLES.find((option) => option.value === style)?.hint;
  }

  protected add() {
    this.create(this.newEvent, () => (this.newEvent = emptyEvent()));
  }

  /** Seçilen fotoğraflar sırayla yüklenir; her biri yüklendikçe listede görünür. */
  protected addPhotos(event: SiteEventListItem, fileInput: HTMLInputElement) {
    const selected = [...(fileInput.files ?? [])];
    const files = selected.slice(0, MAX_PHOTOS - event.photos.length);
    // Aynı dosyalar yeniden seçilebilsin diye sıfırlanır.
    fileInput.value = '';
    if (files.length < selected.length) {
      this.#toast.info(`Bir etkinliğe en fazla ${MAX_PHOTOS} fotoğraf eklenebilir; ilk ${files.length} tanesi yükleniyor.`);
    }
    if (files.length) {
      this.imageBusyId.set(event.id);
      this.#uploadNext(event, files, 0);
    }
  }

  protected removePhoto(event: SiteEventListItem, photo: SiteEventPhoto) {
    if (!confirm('Fotoğraf silinsin mi?')) {
      return;
    }

    this.imageBusyId.set(event.id);
    this.#http.delete(
      `${this.endpoint}/${event.id}/photos/${photo.id}`,
      () => {
        event.photos = event.photos.filter((other) => other.id !== photo.id);
        this.imageBusyId.set(null);
        this.#toast.success('Fotoğraf silindi.');
      },
      () => this.imageBusyId.set(null),
    );
  }

  /** Fotoğrafı bir sola (-1) ya da sağa (+1) taşır. En soldaki kapaktır. */
  protected movePhoto(event: SiteEventListItem, index: number, direction: -1 | 1) {
    const photos = [...event.photos];
    const target = index + direction;
    if (target < 0 || target >= photos.length) {
      return;
    }

    [photos[index], photos[target]] = [photos[target], photos[index]];
    const order: OrderCommand = { ids: photos.map((photo) => photo.id) };

    this.imageBusyId.set(event.id);
    this.#http.put(
      `${this.endpoint}/${event.id}/photos/order`,
      order,
      () => {
        event.photos = photos;
        this.imageBusyId.set(null);
      },
      () => this.imageBusyId.set(null),
    );
  }

  #uploadNext(event: SiteEventListItem, files: File[], uploaded: number) {
    const file = files.shift();
    if (!file) {
      this.imageBusyId.set(null);
      this.#toast.success(uploaded === 1 ? 'Fotoğraf eklendi.' : `${uploaded} fotoğraf eklendi.`);
      return;
    }

    this.#http.upload<SiteEventListItem>(
      `${this.endpoint}/${event.id}/photos`,
      file,
      (updated) => {
        event.photos = updated.photos;
        // Fotoğraf yüklendikçe görünsün diye liste yenilenir; kaydedilmemiş metinler aynı nesnede durur.
        this.items.update((items) => items && [...items]);
        this.#uploadNext(event, files, uploaded + 1);
      },
      () => this.imageBusyId.set(null),
      'post',
    );
  }
}
