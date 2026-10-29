import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  ViewEncapsulation,
} from '@angular/core';
import { SiteService } from '../../services/site';
import { injectNow, timeLeft, timeParts } from '../../utils/countdown';

/** Etkinlikte projeksiyonda gösterilen tam ekran geri sayım. Site menüsü olmadan açılır. */
@Component({
  templateUrl: './counter.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Counter {
  readonly #site = inject(SiteService);
  readonly #now = injectNow();

  protected readonly loaded = computed(() => this.#site.settings() !== null);

  protected readonly countdown = computed(() => {
    const countdown = this.#site.settings()?.countdown;
    const left = countdown ? timeLeft(countdown.endsAt, this.#now()) : null;
    return countdown && left ? { title: countdown.title, parts: timeParts(left) } : null;
  });
}
