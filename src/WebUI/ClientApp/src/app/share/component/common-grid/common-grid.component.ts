
import { Component, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { faAngleDoubleLeft, faAngleDoubleRight, faAngleLeft, faAngleRight, faArrowDown } from '@fortawesome/free-solid-svg-icons';
import { ApplicationUserDto } from 'src/app/web-api-client';
import { PermissionEnum } from '../../constants/enum.constants';
import { AuthorizeService } from 'src/app/authorization/authorize.service';

@Component({
  selector: 'common-grid',
  templateUrl: './common-grid.component.html',
  styleUrls: ['./common-grid.component.scss']
})

export class CommonGridComponent {
  @ViewChild('agGrid') agGrid: AgGridAngular;
  @Input() height;
  @Input() width;
  @Input() rowData = [];
  @Input() columnDefs;
  @Input() isEnableExport = false;
  @Input() canDownload = '';
  @Input() canView = '';
  @Input() page = 1;
  @Input() totalRecords = 0;
  @Input() recordsPerPage = 10;
  @Input() isInitGrid = true;
  @Output() sortEvent = new EventEmitter();
  @Output() rowDoubleClickEvent = new EventEmitter();
  @Output() rowClickEvent = new EventEmitter();
  @Output() pageChangeEvent = new EventEmitter();
  @Output() exportClickedEvent = new EventEmitter();
  private loggedInUser: ApplicationUserDto;
  private permissionEnum = PermissionEnum;
  math = Math;
  gridColumnApi: any;
  faDownload = faArrowDown;
  first = faAngleDoubleLeft;
  last = faAngleDoubleRight;
  prev = faAngleLeft;
  next = faAngleRight;
  defaultColDef = { resizable: true };
  noRowsTemplate = "<div></div>";
  overlayLoadingTemplate = "<div></div>";

  constructor (private authService: AuthorizeService,) { }

  ngOnInit() {
    this.authService.getUser().subscribe((val) => {
      this.loggedInUser = val;
    });
  }

  onGridReady(event) {
    event.api.sizeColumnsToFit();
    this.gridColumnApi = event.columnApi;
  }

  onSort(event) {
    const sortModel = event.api.getSortModel();
    if (sortModel.length > 0) {
      const sortObj = { field: sortModel[0].colId[0].toUpperCase() + sortModel[0].colId.slice(1), sort: sortModel[0].sort };
      this.sortEvent.emit(sortObj);
    } else {
      const sortObj = { field: '', sort: '' };
      this.sortEvent.emit(sortObj);
    }
  }

  onDoubleClickRow(event) {
    if (this.loggedInUser['rolePermission'][this.canView] != this.permissionEnum.Forbidden) {
      this.rowDoubleClickEvent.emit(event);
    } else return;
  }

  clearSort() {
    this.gridColumnApi.applyColumnState({ defaultState: { sort: null } });
  }
}
