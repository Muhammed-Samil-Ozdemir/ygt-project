import { ChangeDetectionStrategy, Component, inject, signal, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  HttpService,
  Icon,
  TeamMemberCommand,
  TeamMemberListItem,
  teamPhotoUrl,
  ToastService,
} from '@ygt/libraries';
import { EditableList } from '../../../utils/editable-list';

@Component({
  imports: [FormsModule, Icon],
  templateUrl: './team.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ContentTeam extends EditableList<TeamMemberListItem, TeamMemberCommand> {
  readonly #http = inject(HttpService);
  readonly #toast = inject(ToastService);

  protected readonly endpoint = '/api/site/content/team';
  protected readonly photoUrl = teamPhotoUrl;
  /** Fotoğrafı yüklenen ya da kaldırılan kişinin ID'si. */
  protected readonly photoBusyId = signal<string | null>(null);
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

  protected uploadPhoto(member: TeamMemberListItem, event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) {
      return;
    }

    this.photoBusyId.set(member.id);
    this.#http.upload<TeamMemberListItem>(
      `${this.endpoint}/${member.id}/photo`,
      file,
      (updated) => {
        // Sadece fotoğraf değişir; karttaki kaydedilmemiş ad ve görev korunur.
        member.photo = updated.photo;
        this.photoBusyId.set(null);
        this.#toast.success('Fotoğraf yüklendi.');
      },
      () => this.photoBusyId.set(null),
    );
  }

  protected removePhoto(member: TeamMemberListItem) {
    if (!confirm(`${member.name} adlı kişinin fotoğrafı kaldırılsın mı?`)) {
      return;
    }

    this.photoBusyId.set(member.id);
    this.#http.delete(
      `${this.endpoint}/${member.id}/photo`,
      () => {
        member.photo = null;
        this.photoBusyId.set(null);
        this.#toast.success('Fotoğraf kaldırıldı.');
      },
      () => this.photoBusyId.set(null),
    );
  }
}
