import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, distinctUntilChanged } from 'rxjs/operators';
import { Location } from '@angular/common';

@Component({
  selector: 'app-header',
  styleUrls: ['./header.scss'],
  templateUrl: './header.html',
})
export class HeaderComponent implements OnInit {
  notHomePage: boolean;
  childpath: string;
  previousUrl = '/';
  constructor (
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private location: Location,
  ) {
    router.events.subscribe((val) => {
      if (val instanceof NavigationEnd) {
        this.childpath = val.url;
      }
    });
  }

  ngOnInit() {
    this.notHomePage = this.router.url !== '/home';
    this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd),
      distinctUntilChanged(),
    ).subscribe(() => {
      this.activatedRoute.root;
      this.notHomePage = this.router.url !== '/home';
    });
  }

  goBack() {
    this.location.back();
  }

  goHome() {
    this.router.navigate(['/home']);
  }
}
