import { ChangeDetectionStrategy, Component, inject, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Icon, siteImageUrl, TeamMemberCommand, TeamMemberListItem, ToastService } from '@ygt/libraries';
import { ImageField } from '../../../components/image-field';
import { EditableList } from '../../../utils/editable-list';

@Component({
  imports: [FormsModule, Icon, ImageField],
  templateUrl: './team.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ContentTeam extends EditableList<TeamMemberListItem, TeamMemberCommand> {
  readonly #toast = inject(ToastService);

  protected readonly endpoint = '/api/site/content/team';
  protected readonly imageUrl = siteImageUrl;
  protected newMember: TeamMemberCommand = { name: '', role: '', isVisible: true };

  protected toCommand(member: TeamMemberListItem): TeamMemberCommand {
    return { name: member.name, role: member.role, isVisible: member.isVisible };
  }

  protected add() {
    this.create(this.newMember, () => {
      this.newMember = { name: '', role: '', isVisible: true };
      this.#toast.info('Fotoğrafı kişinin kartından yükleyebilirsiniz.');
    });
  }

  protected uploadPhoto(member: TeamMemberListItem, file: File) {
    this.uploadImage(member, 'photo', file, (updated) => (member.photo = updated.photo));
  }

  protected removePhoto(member: TeamMemberListItem) {
    this.removeImage(member, 'photo', `${member.name} adlı kişinin fotoğrafı kaldırılsın mı?`, () => (member.photo = null));
  }
}
