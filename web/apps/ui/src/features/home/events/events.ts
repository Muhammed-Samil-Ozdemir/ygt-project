import {
  ChangeDetectionStrategy,
  Component,
  computed,
  ElementRef,
  input,
  signal,
  viewChild,
  ViewEncapsulation,
} from '@angular/core';
import { Icon, SiteEvent, siteImageUrl, SiteTexts } from '@ygt/libraries';

/**
 * Ana sayfadaki etkinlikler, panelde seçilen görünüme göre üç parça hâlinde: polaroid panosu,
 * öne çıkan etkinlikler ve geleneksel etkinlikler. Bir fotoğrafa tıklanınca etkinliğin bütün
 * fotoğrafları pencerede açılır.
 */
@Component({
  selector: 'app-home-events',
  imports: [Icon],
  templateUrl: './events.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeEvents {
  readonly events = input.required<SiteEvent[]>();
  readonly texts = input.required<SiteTexts>();

  protected readonly imageUrl = siteImageUrl;
  protected readonly wall = computed(() => this.events().filter((event) => event.style === 'wall'));
  /** Öne çıkan etkinlikte en fazla üç fotoğraf gösterilir; hepsi pencerede açılır. */
  protected readonly featured = computed(() =>
    this.events()
      .filter((event) => event.style === 'featured')
      .map((event) => ({ event, preview: event.photos.slice(0, 3) })),
  );
  protected readonly recurring = computed(() => this.events().filter((event) => event.style === 'recurring'));

  /** Fotoğrafları pencerede gösterilen etkinlik. */
  protected readonly selected = signal<SiteEvent | null>(null);
  private readonly dialogElement = viewChild.required<ElementRef<HTMLDialogElement>>('dialog');

  protected open(event: SiteEvent) {
    this.selected.set(event);
    this.dialogElement().nativeElement.showModal();
  }

  /** Pencerenin dışındaki karartılmış alana tıklanınca kapanır. */
  protected closeOnBackdrop(event: MouseEvent) {
    if (event.target === this.dialogElement().nativeElement) {
      this.dialogElement().nativeElement.close();
    }
  }
}
