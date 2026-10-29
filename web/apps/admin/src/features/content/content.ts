import { ChangeDetectionStrategy, Component, ViewEncapsulation } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Icon, IconName } from '@ygt/libraries';

/** "Site içeriği" bölümü: üstte sekmeler, altta seçili içerik türünün sayfası. */
@Component({
  imports: [Icon, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './content.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Content {
  protected readonly tabs: { link: string; label: string; icon: IconName }[] = [
    { link: 'texts', label: 'Metinler', icon: 'text' },
    { link: 'about', label: 'Hakkımızda', icon: 'layout' },
    { link: 'team', label: 'Ekip', icon: 'users' },
    { link: 'technologies', label: 'Teknolojiler', icon: 'tag' },
    { link: 'links', label: 'Bağlantılar', icon: 'link' },
  ];
}
