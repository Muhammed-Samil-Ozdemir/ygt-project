import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  ViewEncapsulation,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { LINKS } from '../../constants';
import { SiteService } from '../../services/site';
import { injectNow, timeLeft } from '../../utils/countdown';
import { ABOUT, JOIN_TEXT, TEAM, TECHNOLOGIES } from './home-content';

@Component({
  imports: [RouterLink],
  templateUrl: './home.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Home {
  readonly #site = inject(SiteService);
  readonly #now = injectNow();

  protected readonly links = LINKS;
  protected readonly about = ABOUT;
  protected readonly joinText = JOIN_TEXT;
  protected readonly team = TEAM;
  protected readonly technologies = TECHNOLOGIES;

  /** Admin panelinden geri sayım ayarlanmışsa ve süre dolmamışsa gösterilir. */
  protected readonly countdown = computed(() => {
    const countdown = this.#site.settings()?.countdown;
    const left = countdown ? timeLeft(countdown.endsAt, this.#now()) : null;
    return countdown && left ? { title: countdown.title, left } : null;
  });
}
