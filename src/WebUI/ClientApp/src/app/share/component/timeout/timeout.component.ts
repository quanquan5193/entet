import { Component, HostListener, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthorizeService } from 'src/app/authorization/authorize.service';

@Component({
  selector: 'app-timeout',
  templateUrl: './timeout.component.html',
  styleUrls: ['./timeout.component.scss'],
})
export class TimeoutComponent implements OnInit {

  constructor(
    private authService: AuthorizeService,) {
  }

  ngOnInit() {
  }

  onClick(){
    this.authService.logout();
  }

  @HostListener('window:popstate', ['$event'])
  onPopState(event) {
    this.authService.logout();
  }
}
