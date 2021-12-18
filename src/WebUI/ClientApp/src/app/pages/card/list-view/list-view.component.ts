import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { NgbCalendar, NgbDate, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import * as dayjs from 'dayjs';
import * as $ from 'jquery';
import { distinctUntilChanged, filter, retry } from 'rxjs/operators';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { CONFIG } from 'src/app/share/constants/config.constants';
import { DATA } from 'src/app/share/constants/data.constants';
import { PermissionEnum } from 'src/app/share/constants/enum.constants';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { padNumber, isNumber } from 'src/app/share/factories/ng-bootstrap-date-formatter.factory';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';
import { CommonMethodsService } from 'src/app/share/services/common-methods.service';
import { ApplicationUserDto, CardsClient } from 'src/app/web-api-client';
import { Location } from '@angular/common';

@Component({
  selector: 'app-list-view-card-table',
  templateUrl: './list-view.component.html',
  styleUrls: ['./list-view.component.scss'],
})
export class ListViewCardComponent implements OnInit {
  @ViewChild('commonGrid') commonGrid;
  @ViewChild('closeModal') private closeModalComponent: CloseModalComponent;
  public permission = PERMISSION;
  listStatus = DATA.CardStatusDropdown;
  hoveredDate: NgbDate | null = null;
  fromDate: NgbDate | null;
  toDate: NgbDate | null;
  rowData = [];
  isInitGrid = true;
  columnDefs = [
    { headerName: 'No.', minWidth: 70, maxWidth: 120, cellRenderer: node => this.getRowIndex(node.rowIndex) },
    { field: 'memberNo', headerName: 'お客様番号', minWidth: 140, sortable: true, comparator: () => { } },
    { field: 'createdAt', headerName: '登録日', minWidth: 130, sortable: true, comparator: () => { }, cellRenderer: date => dayjs(date.value).format(CONFIG.DateFormat) },
    { field: 'expiredAt', headerName: '有効期限', minWidth: 120, sortable: true, comparator: () => { }, cellRenderer: dateExp => dayjs(dateExp.value).format(CONFIG.DateShortFormat) },
    { field: 'status', headerName: '状態', minWidth: 50, sortable: true, comparator: () => { }, cellRenderer: status => this.handleFilterStatus(status.value) },
    { field: 'company.companyCode', headerName: '会社コード', minWidth: 50, sortable: true, comparator: () => { } },
    { field: 'store.storeCode', headerName: '店舗コード', minWidth: 50, sortable: true, comparator: () => { } },
    { field: 'createdByName', headerName: '登録者', minWidth: 50, sortable: true, comparator: () => { } }
  ];

  isShowButtonExport = false;
  page = 1;
  totalRecords = 0;
  recordsPerPage = 10;
  sortBy = '';
  sortType = 'asc';
  memberNo = '';
  deviceCode = '';
  expirationDate = '';
  companyCode = '';
  storeCode = '';
  status = null;
  acceptBy = '';

  memberNoExport = '';
  deviceCodeExport = '';
  expirationDateExport = '';
  companyCodeExport = '';
  storeCodeExport = '';
  statusExport = '';
  acceptByExport = '';
  fromDateExport: NgbDate | null;
  toDateExport: NgbDate | null;
  fromDateModel: any;
  toDateModel: any;

  constructor (
    private authService: AuthorizeService,
    private calendar: NgbCalendar,
    private cardClients: CardsClient,
    public formatter: NgbDateParserFormatter,
    private common: CommonMethodsService,
    private translate: TranslateService,
    private route: ActivatedRoute,
    private location: Location,
  ) { }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (Object.keys(params).length) {
        if (params['memberNo']) {
          this.memberNo = params['memberNo'];
        }
        if (params['deviceCode']) {
          this.deviceCode = params['deviceCode'];
        }
        if (params['expirationDate']) {
          this.expirationDate = params['expirationDate'];
        }
        if (params['companyCode']) {
          this.companyCode = params['companyCode'];
        }
        if (params['storeCode']) {
          this.storeCode = params['storeCode'];
        }
        if (params['status']) {
          this.status = params['status'] == 'null' ? null : params['status'];
        }
        if (params['fromDate']) {
          this.fromDate = this.parseDate(params['fromDate']);
          this.fromDateModel = params['fromDate'];
        }
        if (params['toDate']) {
          this.toDate = this.parseDate(params['toDate']);
          this.toDateModel = params['toDate'];
        }
        if (params['acceptBy']) {
          this.acceptBy = params['acceptBy'];
        }
        if (params['page']) {
          this.page = params['page'];
        }
        if (params['sortBy']) {
          this.sortBy = params['sortBy'];
        }
        if (params['sortType']) {
          this.sortType = params['sortType'];
        }
        this.mapDataAndSearch();
      } else {
        this.rowData = [];
        this.isInitGrid = true;
      }
    });
  }

  handleSearch() {
    this.page = 1;
    this.pushSearchToParam();
  }

  pushSearchToParam() {
    const param = {
      memberNo: this.memberNo,
      deviceCode: this.deviceCode,
      expirationDate: this.expirationDate,
      companyCode: this.companyCode,
      storeCode: this.storeCode,
      status: this.status,
      acceptBy: this.acceptBy,
      fromDate: this.fromDate ? this.formatDate(this.fromDate) : '',
      toDate: this.toDate ? this.formatDate(this.toDate) : '',
      page: this.page,
      sortBy: this.sortBy,
      sortType: this.sortType
    };
    this.authService.navigateToUrlWithParam(`/card`, param);
  }

  mapDataAndSearch() {
    this.memberNoExport = this.memberNo;
    this.deviceCodeExport = this.deviceCode;
    this.expirationDateExport = this.expirationDate;
    this.companyCodeExport = this.companyCode;
    this.storeCodeExport = this.storeCode;
    this.statusExport = this.status;
    this.acceptByExport = this.acceptBy;
    this.fromDateExport = this.fromDate;
    this.toDateExport = this.toDate;
    this.load();
  }

  handleClearData() {
    this.authService.navigateToUrlWithParam(`/card`, null)
    this.rowData = [];
    this.totalRecords = 0;
    this.sortBy = '';
    this.sortType = 'asc';
    this.memberNo = '';
    this.deviceCode = '';
    this.expirationDate = '';
    this.companyCode = '';
    this.storeCode = '';
    this.status = null;
    this.acceptBy = '';
    this.fromDate = null;
    this.toDate = null;
    this.isShowButtonExport = false;
    this.memberNoExport = '';
    this.deviceCodeExport = '';
    this.expirationDateExport = '';
    this.companyCodeExport = '';
    this.storeCodeExport = '';
    this.statusExport = '';
    this.acceptByExport = '';
    this.fromDateExport = null;
    this.toDateExport = null;
    this.isInitGrid = true;
    this.commonGrid.clearSort();
    
  }

  getRowIndex(index) {
    return (index + 1) + ((this.page - 1) * this.recordsPerPage);
  }

  load() {
    this.cardClients.getCardsWithPagination(
      this.sortBy,
      this.sortType,
      this.page.toString(),
      this.recordsPerPage.toString(),
      this.memberNoExport,
      this.deviceCodeExport,
      this.expirationDateExport,
      this.companyCodeExport,
      this.storeCodeExport,
      this.statusExport,
      this.formatDate(this.fromDateExport),
      this.formatDate(this.toDateExport),
      this.acceptByExport)
      .subscribe(result => {
        this.totalRecords = result.totalCount;
        this.rowData = result.items;
        this.isShowButtonExport = this.rowData.length > 0;
        this.isInitGrid = false;
      }, () => this.closeModalComponent.open(this.translate.instant('system.error.fetchDataFail')));
  }

  handleQueryDate(date: Date) {
    return new Date(date.getTime() - (date.getTimezoneOffset() * 60000));
  }

  handleFilterStatus(status) {
    switch (status) {
      case DATA.CardStatus.Unissued:
        return '未発行';
      case DATA.CardStatus.Issued:
        return '発行済';
      case DATA.CardStatus.Missing:
        return '紛失';
      case DATA.CardStatus.Disposal:
        return '廃棄';
      case DATA.CardStatus.Withdrawal:
        return '退会';
    }
  }

  onSortCommon(event) {
    if (this.totalRecords == 0) {
      this.commonGrid.clearSort();
      return;
    }

    if (event.field !== null || event.field !== undefined) {
      this.sortBy = event.field;
      this.sortType = event.sort;
    }

    if (event.field == '' && this.rowData.length == 0) {
      return;
    }

    this.load();
  }
  2;
  onRowDoubleClickCommon(event) {
    this.authService.navigateToUrlWithoutReplace(`/card/${event.id}`);
  }

  onRowClickCommon(event) {
  }

  onPageChangeEvent(event) {
    if (this.page != event) {
      this.page = event;
      this.load();
    }
  }

  onExportClickedEvent(event) {
    this.cardClients.getCardsExport(
      this.memberNoExport,
      this.deviceCodeExport,
      this.expirationDateExport,
      this.companyCodeExport,
      this.storeCodeExport,
      this.statusExport,
      this.fromDateExport == null ? null :  this.formatDate(this.fromDateExport),
      this.toDateExport == null ? null :  this.formatDate(this.toDateExport),
      this.acceptByExport)
      .subscribe(result => {
        const blobContent = result.data;
        const fileSaveName = 'プリペイドカード保管情報＿';
        const extension = result.fileName.substring(result.fileName.lastIndexOf('.') + 1);
        const currentDate = dayjs().format('YYYYMMDD＿HHmmss');
        const fileName = `${fileSaveName}${currentDate}.${extension}`;
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
          const newBlob = new Blob([blobContent], {
            type: this.common.getMimeType(this.getFileType(fileName)),
          });
          window.navigator.msSaveOrOpenBlob(newBlob, fileName);
          return;
        } else {
          const newFile = new File([blobContent], fileName);
          const data = window.URL.createObjectURL(newFile);
          const link = document.createElement("a");
          link.href = data;
          link.download = fileName;
          link.click();
        }
      }, (err) => {
        const errorResponse = err.response;
        const errorResponseJson = JSON.parse(errorResponse);
        if (errorResponseJson.status === 400 && errorResponseJson.errors['MoreThanRecord']) {
          this.closeModalComponent.open(this.translate.instant('card.exportMoreThanFirst') + errorResponseJson.errors['MoreThanRecord'][0] + this.translate.instant('card.exportMoreThanSecond'));
        } else {
          this.closeModalComponent.open(this.translate.instant('system.error.fetchDataFail'));
        }
      });
  }

  getFileType(attachment) {
    return this.split(attachment.fileType, ".")[1];
  }

  split(val, separator) {
    const separatorNew = separator.replace(/([^\w\s])/g, "\\$1");
    return val.split(new RegExp(separatorNew));
  }

  formatDate(date: NgbDateStruct): string {
    if (date) return '' + date.year + '/' + (date.month <= 9 ? '0' + date.month : date.month) + '/' + (date.day <= 9 ? '0' + date.day : date.day);
    return null;
  }

  parseDate(date: string): NgbDate {
    if (date) {
      let dateSplit = date.split('/');
      return {
        year: +dateSplit[0],
        month: +dateSplit[1],
        day: +dateSplit[2]
      } as NgbDate;
    }
    return null;
  }

  onDateSelection(date: NgbDate) {
    if (!this.fromDate && !this.toDate) {
      this.fromDate = date;
    } else if (this.fromDate && !this.toDate && date && (date.after(this.fromDate) || date.equals(this.fromDate))) {
      this.toDate = date;
    } else {
      this.toDate = null;
      this.fromDate = date;
    }
  }

  validateFromDate(date) {
    const parsed = this.formatter.parse(date.target.value);
    if (parsed == null) {
      this.fromDateModel = null;
      this.fromDate = null;
      return;
    }
    if (parsed.year < 1900 || parsed.year > 2100) {
      this.fromDateModel = null;
      this.fromDate = null;
      return;
    };
    if (parsed.day == null) {
      this.fromDateModel = null;
      this.fromDate = null;
      return;
    }
    const numbers = date.target.value.trim().split('/');
    if (!(+numbers[2])) {
      this.fromDateModel = null;
      this.fromDate = null;
      return;
    }
  }

  validateToDate(date) {
    const parsed = this.formatter.parse(date.target.value);
    if (parsed == null) {
      this.toDateModel = null;
      this.toDate = null;
      return;
    }
    if (parsed.year < 1900 || parsed.year > 2100) {
      this.toDateModel = null;
      this.toDate = null;
      return;
    };
    if (parsed.day == null) {
      this.toDateModel = null;
      this.toDate = null;
      return;
    }
    const numbers = date.target.value.trim().split('/');
    if (!(+numbers[2])) {
      this.toDateModel = null;
      this.toDate = null;
      return;
    }
  }

  validateExpirationDate(date) {
    const dateParts = date.trim().split("/");
    if (dateParts.length != 2) {
      if (dateParts.length == 1 && isNumber(dateParts[0]) && +dateParts[0] <= 2100 && +dateParts[0] >= 1900) {
        this.expirationDate = `${dateParts[0]}`;
      } else {
        this.expirationDate = '';
      }
      return;
    }
    if (isNumber(dateParts[0]) && isNumber(dateParts[1]) && +dateParts[1] <= 12 && +dateParts[1] >= 1 && +dateParts[0] <= 2100 && +dateParts[0] >= 1900) {
      this.expirationDate = `${dateParts[0]}/${padNumber(dateParts[1])}`;
      return;
    } else {
      this.expirationDate = '';
      return;
    }
  }

  isHovered(date: NgbDate) {
    return this.fromDate && !this.toDate && this.hoveredDate && date.after(this.fromDate) && date.before(this.hoveredDate);
  }

  isInside(date: NgbDate) {
    return this.toDate && date.after(this.fromDate) && date.before(this.toDate);
  }

  isRange(date: NgbDate) {
    return date.equals(this.fromDate) || (this.toDate && date.equals(this.toDate)) || this.isInside(date) || this.isHovered(date);
  }

  validateInput(currentValue: NgbDate | null, input: string): NgbDate | null {
    const parsed = this.formatter.parse(input);
    return parsed && this.calendar.isValid(NgbDate.from(parsed)) ? NgbDate.from(parsed) : currentValue;
  }

}
