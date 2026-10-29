import { ChangeDetectionStrategy, Component, computed, inject, ViewEncapsulation } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Icon } from '@ygt/libraries';
import { SiteService } from '../services/site';

/** Başvuru gönderildikten sonra görünen teşekkür bölümü. Metin panelden ("Başvurular" grubu) gelir. */
@Component({
  selector: 'app-application-sent',
  imports: [Icon, RouterLink],
  template: `
    <div class="result">
      <span class="result-icon result-icon-success"><ygt-icon name="check" /></span>
      <h1>Başvurunuz alındı</h1>
      <p class="muted pre-line">{{ thanks() }}</p>
      <a class="btn btn-primary" routerLink="/">Ana sayfaya dön</a>
    </div>
  `,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationSent {
  readonly #site = inject(SiteService);

  protected readonly thanks = computed(() => this.#site.content()?.texts.applicationThanks ?? '');
}
