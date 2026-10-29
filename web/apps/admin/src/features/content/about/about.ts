import { ChangeDetectionStrategy, Component, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AboutCardCommand, AboutCardListItem, Icon, siteImageUrl } from '@ygt/libraries';
import { ImageField } from '../../../components/image-field';
import { EditableList } from '../../../utils/editable-list';

@Component({
  imports: [FormsModule, Icon, ImageField],
  templateUrl: './about.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ContentAbout extends EditableList<AboutCardListItem, AboutCardCommand> {
  protected readonly endpoint = '/api/site/content/about';
  protected readonly imageUrl = siteImageUrl;
  protected newCard: AboutCardCommand = { title: '', text: '', isVisible: true };

  protected toCommand(card: AboutCardListItem): AboutCardCommand {
    return { title: card.title, text: card.text, isVisible: card.isVisible };
  }

  protected add() {
    this.create(this.newCard, () => (this.newCard = { title: '', text: '', isVisible: true }));
  }

  protected uploadCardImage(card: AboutCardListItem, file: File) {
    this.uploadImage(card, 'image', file, (updated) => (card.image = updated.image));
  }

  protected removeCardImage(card: AboutCardListItem) {
    this.removeImage(card, 'image', `“${card.title}” kartının görseli kaldırılsın mı?`, () => (card.image = null));
  }
}
