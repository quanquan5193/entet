import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { PermissionEnum } from 'src/app/share/constants/enum.constants';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { ApplicationUserDto } from 'src/app/web-api-client';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  public permission = PERMISSION;
  constructor () { }

  ngOnInit() { }
}
