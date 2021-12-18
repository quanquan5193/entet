import { Component } from '@angular/core';
import { AuthorizeService } from 'src/app/authorization/authorize.service';

@Component({
  selector: 'app-not-found',
  styleUrls: ['./not-found.component.scss'],
  templateUrl: './not-found.component.html',
})
export class NotFoundComponent {

  constructor(
    private authService: AuthorizeService,
  ) {
  }

  goToHome() {
    this.authService.navigateToUrl('/');
  }
}
