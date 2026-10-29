import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  OnInit,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  errorMessage,
  Form,
  HackathonApplicationCommand,
  HttpService,
  Icon,
  ToastService,
} from '@ygt/libraries';
import { ApplicationSent } from '../../components/application-sent';
import { SiteService } from '../../services/site';

@Component({
  imports: [ApplicationSent, FormsModule, Icon, RouterLink],
  templateUrl: './hackathon-application.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class HackathonApplication implements OnInit {
  readonly #http = inject(HttpService);
  readonly #router = inject(Router);
  readonly #toast = inject(ToastService);
  readonly #site = inject(SiteService);

  /** Aydınlatma metni panelden ("Başvurular" grubu) gelir. null: yükleniyor, "": girilmemiş. */
  protected readonly kvkkText = computed(() => this.#site.content()?.texts.kvkkText ?? null);
  protected readonly form = signal<Form | null>(null);
  protected readonly closedMessage = signal<string | null>(null);
  protected readonly sending = signal(false);
  protected readonly sent = signal(false);

  protected kvkkApproved = false;
  protected model: HackathonApplicationCommand = {
    teamName: '',
    member1: { name: '', phone: '', email: '' },
    member2: { name: '', phone: '', email: '' },
    member3: { name: '', phone: '', email: '' },
  };

  protected readonly members = [
    { key: 'member1', title: 'Takım kaptanı', optional: false },
    { key: 'member2', title: 'İkinci üye', optional: false },
    { key: 'member3', title: 'Üçüncü üye', optional: true },
  ] as const;

  ngOnInit() {
    this.#http.get<Form>(
      '/api/forms/active',
      (form) => {
        // Aktif form bireysel başvuru alıyorsa ona yönlendirilir.
        if (form.type !== 'hackathon') {
          this.#router.navigateByUrl('/form', { replaceUrl: true });
          return;
        }
        this.form.set(form);
      },
      (err) => this.closedMessage.set(errorMessage(err)),
      { quiet: true },
    );
  }

  protected submit() {
    const form = this.form();
    if (!form) {
      return;
    }

    if (!this.kvkkApproved) {
      this.#toast.error('Başvuru için aydınlatma metnini onaylamanız gerekiyor.');
      return;
    }

    this.sending.set(true);
    this.#http.post(
      `/api/forms/${form.id}/hackathon-applications`,
      this.model,
      () => {
        this.sending.set(false);
        this.sent.set(true);
      },
      () => this.sending.set(false),
    );
  }
}
