import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { DevicesClient } from 'src/app/web-api-client';

@Component({
  selector: 'app-list-view-device-management-table',
  templateUrl: './list-view.component.html',
  styleUrls: ['./list-view.component.scss'],
})
export class ListViewDeviceManagementComponent implements OnInit {
  @ViewChild('commonGrid') commonGrid;

  public permission = PERMISSION;
  isInitGrid = true;
  isClickSearch = false;
  rowData: any = [];
  page = 1;
  totalRecords = 0;
  companyCode: string;
  companyName: string;
  deviceCode: string;

  companyCodeFilter: string;
  companyNameFilter: string;
  deviceCodeFilter: string;

  recordsPerPage = 10;
  orderBy = 'Id';
  orderType = 'desc';
  isShowButtonExport = false;
  columnDefs = [
    { field: 'id', hide: true },
    { field: 'no', headerName: 'No.', minWidth: 70, maxWidth: 90, cellRenderer: node => this.getRowIndex(node.rowIndex) },
    { field: 'deviceCode', headerName: '端末番号', sortable: true, comparator: () => { } },
    { field: 'companyCode', headerName: '会社コード', sortable: true, comparator: () => { } },
    { field: 'normalizedCompanyName', headerName: '会社名称', sortable: true, comparator: () => { } },
    { field: 'storeCode', headerName: '店舗コード', sortable: true, comparator: () => { } },
    { field: 'normalizedStoreName', headerName: '店舗名称', sortable: true, comparator: () => { } },
    {
      field: 'createdAt', headerName: '登録日', sortable: true, comparator: () => { }, valueFormatter: function (params) {
        var dateObj = params.value;
        var day = dateObj.getDate();
        var month = dateObj.getMonth() + 1;
        var year = dateObj.getFullYear();
        return `${year}/${month}/${day}`;
      }
    },
    { field: 'status', headerName: '状態', sortable: true, comparator: () => { } }
  ];
  constructor (
    private authService: AuthorizeService,
    private devicesClient: DevicesClient,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (Object.keys(params).length) {
        if (params['companyName']) {
          this.companyName = params['companyName'];
        }
        if (params['companyCode']) {
          this.companyCode = params['companyCode'];
        }
        if (params['deviceCode']) {
          this.deviceCode = params['deviceCode'];
        }
        if (params['page']) {
          this.page = params['page'];
        }
        if (params['orderBy']) {
          this.orderBy = params['orderBy'];
        }
        if (params['orderType']) {
          this.orderType = params['orderType'];
        }
        this.mapDataAndSearch();
      } else {
        this.rowData = [];
        this.isInitGrid = true;
      }
    });
  }

  getRowIndex(index) {
    return (index + 1) + ((this.page - 1) * this.recordsPerPage);
  }

  onSortCommon(event) {

    if (this.totalRecords == 0) {
      this.commonGrid.clearSort();
      return;
    }

    if (event.field !== null || event.field !== undefined) {
      this.orderBy = event.field;
      this.orderType = event.sort;
    }

    if (event.field == '' && this.rowData.length == 0) {
      return;
    }

    this.searchData();
  }

  onRowDoubleClickCommon(event) {
    this.authService.navigateToUrlWithoutReplace(`/system-management/device-management/${event.id}/edit`);
  }

  handleClickAddNew() {
    this.authService.navigateToUrlWithoutReplace(`/system-management/device-management/create`);
  }

  onRowClickCommon(event) {
  }

  onPageChangeEvent(event) {
    if (this.page != event) {
      this.page = event;
      this.searchData();
    }
  }

  handleSearchData() {
    this.isClickSearch = true;
    this.page = 1;
    this.pushSearchToParam();
  }

  pushSearchToParam() {
    const param = {
      companyName: this.companyName,
      companyCode: this.companyCode,
      deviceCode: this.deviceCode,
      page: this.page,
      orderBy: this.orderBy,
      orderType: this.orderType
    };
    this.authService.navigateToUrlWithParam(`/system-management/device-management`, param);
  }

  mapDataAndSearch(): void {
    this.companyCodeFilter = this.companyCode;
    this.companyNameFilter = this.companyName;
    this.deviceCodeFilter = this.deviceCode;
    this.searchData();
  }

  searchData() {
    this.devicesClient.searchDevicesWithPagination(this.companyCodeFilter, this.companyNameFilter, this.deviceCodeFilter, this.orderBy, this.orderType, this.page, this.recordsPerPage)
      .subscribe(res => {
        this.rowData = res.items;
        this.totalRecords = res.totalCount;
        this.isInitGrid = false;
      });
  }

  onExportClickedEvent(event) {
  }

  toASCII(event) {
    if (!event || !event.target || !event.target.value || event.target.value.length < 0) return '';
    let chars = event.target.value;
    let ascii = '';
    for (let i = 0, l = chars.length; i < l; i++) {
      let c = chars[i].charCodeAt(0);

      // make sure we only convert half-full width char
      if (c >= 0xFF00 && c <= 0xFFEF) {
        c = 0xFF & (c + 0x20);
      }

      ascii += String.fromCharCode(c);
    }

    return ascii;
  }

}
