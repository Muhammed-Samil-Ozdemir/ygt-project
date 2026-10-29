import { ChangeDetectionStrategy, Component, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Icon, SiteLinkCommand, SiteLinkListItem } from '@ygt/libraries';
import { EditableList } from '../../../utils/editable-list';

@Component({
  imports: [FormsModule, Icon],
  templateUrl: './links.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ContentLinks extends EditableList<SiteLinkListItem, SiteLinkCommand> {
  protected readonly endpoint = '/api/site/content/links';
  protected newLink: SiteLinkCommand = { label: '', url: '', isVisible: true };

  protected toCommand(link: SiteLinkListItem): SiteLinkCommand {
    return { label: link.label, url: link.url, isVisible: link.isVisible };
  }

  protected add() {
    this.create(this.newLink, () => (this.newLink = { label: '', url: '', isVisible: true }));
  }
}
