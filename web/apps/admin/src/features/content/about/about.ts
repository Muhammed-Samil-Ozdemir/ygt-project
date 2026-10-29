import { ChangeDetectionStrategy, Component, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AboutCardCommand, AboutCardListItem, Icon } from '@ygt/libraries';
import { EditableList } from '../../../utils/editable-list';

@Component({
  imports: [FormsModule, Icon],
  templateUrl: './about.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ContentAbout extends EditableList<AboutCardListItem, AboutCardCommand> {
  protected readonly endpoint = '/api/site/content/about';
  protected newCard: AboutCardCommand = { title: '', text: '', isVisible: true };

  protected toCommand(card: AboutCardListItem): AboutCardCommand {
    return { title: card.title, text: card.text, isVisible: card.isVisible };
  }

  protected add() {
    this.create(this.newCard, () => (this.newCard = { title: '', text: '', isVisible: true }));
  }
}
