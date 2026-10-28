import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastContainer } from '@ygt/libraries';

@Component({
  imports: [RouterOutlet, ToastContainer],
  selector: 'app-root',
  template: '<router-outlet /><ygt-toasts />',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {}
