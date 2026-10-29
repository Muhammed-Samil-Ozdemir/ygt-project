import { ChangeDetectionStrategy, Component, computed, input, ViewEncapsulation } from '@angular/core';

/** Metnin ilk kelimesini YGT turuncusuyla yazar: "Yazılım Geliştirme Topluluğu" -> turuncu "Yazılım". */
@Component({
  selector: 'app-accent-text',
  template: `<span class="brand-accent">{{ parts()[0] }}</span>{{ parts()[1] }}`,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AccentText {
  readonly text = input.required<string>();

  protected readonly parts = computed(() => {
    const text = this.text();
    const space = text.indexOf(' ');
    return space < 0 ? [text, ''] : [text.slice(0, space), text.slice(space)];
  });
}
