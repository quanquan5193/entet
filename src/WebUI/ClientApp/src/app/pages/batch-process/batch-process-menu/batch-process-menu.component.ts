import { Component, OnInit } from '@angular/core';
import { PERMISSION } from 'src/app/share/constants/permission.constants';

@Component({
  selector: 'app-batch-process-menu',
  templateUrl: './batch-process-menu.component.html',
  styleUrls: ['./batch-process-menu.component.scss'],
})
export class BatchProcessMenuComponent implements OnInit {
  public permission = PERMISSION;
  constructor () { }

  ngOnInit() { }
}
