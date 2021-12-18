import { Component, OnInit } from '@angular/core';
import { PERMISSION } from 'src/app/share/constants/permission.constants';

@Component({
  selector: 'app-system-management-menu',
  templateUrl: './system-management-menu.component.html',
  styleUrls: ['./system-management-menu.component.scss'],
})
export class SystemManagementMenuComponent implements OnInit {
  public permission = PERMISSION;
  constructor () { }

  ngOnInit() {

  }
}
