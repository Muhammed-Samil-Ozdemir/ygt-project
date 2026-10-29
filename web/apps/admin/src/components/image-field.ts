import {
  ChangeDetectionStrategy,
  Component,
  input,
  output,
  ViewEncapsulation,
} from '@angular/core';
import { Icon } from '@ygt/libraries';

/**
 * Görsel alanı: önizleme, "Yükle / Değiştir" ve "Kaldır" butonları. Dosyayı sadece seçtirir;
 * yükleme ve silme isteğini kullanan sayfa yapar.
 */
@Component({
  selector: 'app-image-field',
  imports: [Icon],
  template: `
    <div class="image-field-preview">
      @if (src(); as src) {
        <img [src]="src" [alt]="alt()" />
      } @else {
        <ygt-icon name="image" />
      }
    </div>
    <div class="image-field-actions">
      <input
        #fileInput
        type="file"
        accept="image/png,image/jpeg,image/webp"
        hidden
        (change)="pick(fileInput)"
      />
      <button type="button" class="btn btn-outline btn-sm" [disabled]="busy()" (click)="fileInput.click()">
        {{ busy() ? 'Yükleniyor...' : src() ? 'Değiştir' : 'Görsel yükle' }}
      </button>
      @if (src()) {
        <button type="button" class="btn btn-ghost btn-sm" [disabled]="busy()" (click)="remove.emit()">Kaldır</button>
      }
    </div>
  `,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    class: 'image-field',
    '[class.image-field-wide]': "shape() === 'wide'",
    '[class.image-field-contain]': "shape() === 'contain'",
  },
})
export class ImageField {
  /** Görselin adresi; görsel yoksa null. */
  readonly src = input<string | null>(null);
  readonly alt = input('');
  readonly busy = input(false);
  /** square: ekip fotoğrafı; wide: kart görseli; contain: logo gibi kırpılmadan gösterilecek görsel. */
  readonly shape = input<'square' | 'wide' | 'contain'>('square');

  readonly upload = output<File>();
  readonly remove = output<void>();

  protected pick(fileInput: HTMLInputElement) {
    const file = fileInput.files?.[0];
    // Aynı dosya yeniden seçilebilsin diye sıfırlanır.
    fileInput.value = '';
    if (file) {
      this.upload.emit(file);
    }
  }
}
