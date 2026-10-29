import { ChangeDetectionStrategy, Component, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Icon, TechnologyCommand, TechnologyListItem } from '@ygt/libraries';
import { EditableList } from '../../../utils/editable-list';

@Component({
  imports: [FormsModule, Icon],
  templateUrl: './technologies.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ContentTechnologies extends EditableList<TechnologyListItem, TechnologyCommand> {
  protected readonly endpoint = '/api/site/content/technologies';
  protected newName = '';

  protected toCommand(technology: TechnologyListItem): TechnologyCommand {
    return { name: technology.name };
  }

  protected add() {
    this.create({ name: this.newName }, () => (this.newName = ''));
  }
}
