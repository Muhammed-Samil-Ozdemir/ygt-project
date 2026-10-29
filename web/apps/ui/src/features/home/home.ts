import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  ViewEncapsulation,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { Icon, teamPhotoUrl } from '@ygt/libraries';
import { AccentText } from '../../components/accent-text';
import { SiteService } from '../../services/site';
import { injectNow, timeLeft, timeParts } from '../../utils/countdown';

@Component({
  imports: [AccentText, Icon, RouterLink],
  templateUrl: './home.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Home {
  readonly #site = inject(SiteService);
  readonly #now = injectNow();

  /** Metinler, kartlar, ekip ve teknolojiler panelden yönetilir; yüklenene kadar null. */
  protected readonly content = this.#site.content;
  protected readonly photoUrl = teamPhotoUrl;

  /** Admin panelinden geri sayım ayarlanmışsa ve süre dolmamışsa gösterilir. */
  protected readonly countdown = computed(() => {
    const countdown = this.#site.settings()?.countdown;
    const left = countdown ? timeLeft(countdown.endsAt, this.#now()) : null;
    return countdown && left ? { title: countdown.title, parts: timeParts(left) } : null;
  });

  /** Fotoğrafı olmayan kişi için baş harfler: "Ayşe Melek Acar" -> "AA". */
  protected initials(name: string) {
    const words = name.trim().split(/\s+/);
    return (words[0][0] + (words.length > 1 ? words[words.length - 1][0] : '')).toLocaleUpperCase('tr');
  }

  /** Kart numarası: 1 -> "01". */
  protected number(index: number) {
    return (index + 1).toString().padStart(2, '0');
  }
}
