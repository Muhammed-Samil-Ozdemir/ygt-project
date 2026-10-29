import { Directive, inject, OnInit, signal } from '@angular/core';
import { HttpService, OrderCommand, ToastService } from '@ygt/libraries';

/**
 * Panelde yerinde düzenlenen sıralı liste: hakkımızda kartları, ekip, teknolojiler, bağlantılar.
 * Her kayıt kendi formunda düzenlenir ve "Kaydet" ile gönderilir. Ekleme, silme ve sıralama listeyi
 * yeniden yüklemeden yapılır; böylece diğer kayıtlardaki kaydedilmemiş değişiklikler kaybolmaz.
 */
@Directive()
export abstract class EditableList<TItem extends { id: string }, TCommand> implements OnInit {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);
  /** Kayıtların sunucudaki hâli (komut olarak); değişiklik kontrolünde karşılaştırılır. */
  readonly #saved = new Map<string, string>();

  /** null: yükleniyor. */
  protected readonly items = signal<TItem[] | null>(null);
  /** İşlem süren kaydın ID'si; o kaydın butonları kilitlenir. */
  protected readonly busyId = signal<string | null>(null);
  /** Görseli yüklenen ya da kaldırılan kaydın ID'si. */
  protected readonly imageBusyId = signal<string | null>(null);
  protected readonly adding = signal(false);
  /** Sürüklenen kaydın sırası ve üzerine gelinen sıra (bırakılacak yeri göstermek için). */
  protected readonly dragIndex = signal<number | null>(null);
  protected readonly dropIndex = signal<number | null>(null);

  /** API adresi, ör. "/api/site/content/about". */
  protected abstract readonly endpoint: string;

  /** Kaydın düzenlenebilen alanları: kaydederken gönderilir. */
  protected abstract toCommand(item: TItem): TCommand;

  ngOnInit() {
    this.#http.get<TItem[]>(this.endpoint, (items) => {
      this.#remember(...items);
      this.items.set(items);
    });
  }

  protected isDirty(item: TItem) {
    return this.#saved.get(item.id) !== JSON.stringify(this.toCommand(item));
  }

  protected save(item: TItem) {
    this.busyId.set(item.id);
    this.#http.put(
      `${this.endpoint}/${item.id}`,
      this.toCommand(item),
      () => {
        this.#remember(item);
        this.busyId.set(null);
        this.#toast.success('Kaydedildi. Değişiklik site yenilenince görünür.');
      },
      () => this.busyId.set(null),
    );
  }

  /** Yeni kaydı listenin sonuna ekler. Başarılı olursa done çağrılır (ör. formu temizlemek için). */
  protected create(command: TCommand, done: () => void) {
    this.adding.set(true);
    this.#http.post<TItem>(
      this.endpoint,
      command,
      (item) => {
        this.#remember(item);
        this.items.update((items) => [...(items ?? []), item]);
        this.adding.set(false);
        this.#toast.success('Eklendi.');
        done();
      },
      () => this.adding.set(false),
    );
  }

  protected remove(item: TItem, question: string) {
    if (!confirm(question)) {
      return;
    }

    this.busyId.set(item.id);
    this.#http.delete(
      `${this.endpoint}/${item.id}`,
      () => {
        this.#saved.delete(item.id);
        this.items.update((items) => items?.filter((other) => other.id !== item.id) ?? null);
        this.busyId.set(null);
        this.#toast.success('Silindi.');
      },
      () => this.busyId.set(null),
    );
  }

  /** Kaydı bir yukarı (-1) ya da aşağı (+1) taşır. Klavyeyle ve dokunmatik ekranda sıralama bununla yapılır. */
  protected move(index: number, direction: -1 | 1) {
    this.#reorder(index, index + direction);
  }

  /**
   * Sürükle-bırak: sürükleme kaydın tutamacından başlar (".drag-handle"), kaydın kutusu (".drag-item")
   * bırakma alanıdır. Kutunun tamamı sürüklenebilir olsaydı içindeki metin kutularında yazı seçilemezdi.
   */
  protected dragStart(index: number, event: DragEvent) {
    if (this.busyId() !== null || !event.dataTransfer) {
      event.preventDefault();
      return;
    }

    // Sürüklerken sadece tutamaç değil, kaydın tamamı görünsün.
    const item = (event.target as HTMLElement).closest<HTMLElement>('.drag-item');
    if (item) {
      event.dataTransfer.setDragImage(item, 24, 24);
    }
    event.dataTransfer.effectAllowed = 'move';
    event.dataTransfer.setData('text/plain', '');
    this.dragIndex.set(index);
  }

  protected dragOver(index: number, event: DragEvent) {
    if (this.dragIndex() === null) {
      return;
    }

    event.preventDefault();
    this.dropIndex.set(index);
  }

  protected drop(index: number, event: DragEvent) {
    event.preventDefault();
    const from = this.dragIndex();
    this.endDrag();

    if (from !== null) {
      this.#reorder(from, index);
    }
  }

  protected endDrag() {
    this.dragIndex.set(null);
    this.dropIndex.set(null);
  }

  /** Kaydı from sırasından to sırasına taşır ve yeni sırayı kaydeder. */
  #reorder(from: number, to: number) {
    const items = [...(this.items() ?? [])];
    if (from === to || to < 0 || to >= items.length) {
      return;
    }

    const [moved] = items.splice(from, 1);
    items.splice(to, 0, moved);
    const order: OrderCommand = { ids: items.map((item) => item.id) };

    this.busyId.set(moved.id);
    this.#http.put(
      `${this.endpoint}/order`,
      order,
      () => {
        this.items.set(items);
        this.busyId.set(null);
      },
      () => this.busyId.set(null),
    );
  }

  /**
   * Kaydın görselini yükler (ekip fotoğrafı, kart görseli). path: API'deki alt yol, ör. "photo".
   * Sadece görsel değişir (apply); kayıttaki kaydedilmemiş diğer değişiklikler korunur.
   */
  protected uploadImage(item: TItem, path: string, file: File, apply: (updated: TItem) => void) {
    this.imageBusyId.set(item.id);
    this.#http.upload<TItem>(
      `${this.endpoint}/${item.id}/${path}`,
      file,
      (updated) => {
        apply(updated);
        this.imageBusyId.set(null);
        this.#toast.success('Görsel yüklendi.');
      },
      () => this.imageBusyId.set(null),
    );
  }

  protected removeImage(item: TItem, path: string, question: string, apply: () => void) {
    if (!confirm(question)) {
      return;
    }

    this.imageBusyId.set(item.id);
    this.#http.delete(
      `${this.endpoint}/${item.id}/${path}`,
      () => {
        apply();
        this.imageBusyId.set(null);
        this.#toast.success('Görsel kaldırıldı.');
      },
      () => this.imageBusyId.set(null),
    );
  }

  #remember(...items: TItem[]) {
    for (const item of items) {
      this.#saved.set(item.id, JSON.stringify(this.toCommand(item)));
    }
  }
}
