import { RequestsReceiptedsClient, RequestTypeDto } from './../../../web-api-client';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbCalendar, NgbDate, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { CardsClient } from 'src/app/web-api-client';
import { TranslateService } from '@ngx-translate/core';
import { CommonMethodsService } from 'src/app/share/services/common-methods.service';
import * as dayjs from 'dayjs';
import { CONFIG } from 'src/app/share/constants/config.constants';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-list-view-history-inquiry-table',
  templateUrl: './list-view.component.html',
  styleUrls: ['./list-view.component.scss'],
})
export class ListViewHistoryInquiryComponent implements OnInit {
  @ViewChild('commonGrid') commonGrid;
  @ViewChild('closeModal') private closeModalComponent: CloseModalComponent;
  public permission = PERMISSION;
  hoveredDate: NgbDate | null = null;
  fromDate: NgbDate | null;
  toDate: NgbDate | null;
  fromDateModel: any;
  toDateModel: any;
  memberNo: string;
  requestTypes: RequestTypeDto[];
  requestTypeCode: number;
  companyName: string;
  storeName: string;
  deviceCode: string;
  createdBy: string;
  rowData: any = [];
  page = 1;
  totalRecords = 0;
  recordsPerPage = 0;
  orderBy: string;
  orderType: string;
  isShowButtonExport = false;
  isInitGrid = true;
  columnDefs = [
    { field: 'id', hide: true },
    { field: 'index', headerName: 'No.', minWidth: 70, maxWidth: 90, comparator: () => { } },
    {
      field: 'receiptedDatetime', headerName: '受付日', sortable: true, comparator: () => { }, cellRenderer: date => dayjs(date.value).format(CONFIG.DateFormat)
    },
    { field: 'requestTypeName', headerName: '受付区分', sortable: true, comparator: () => { } },
    { field: 'memberNo', headerName: 'お客様番号', sortable: true, comparator: () => { } },
    { field: 'memberName', headerName: 'お客様氏名', sortable: true, comparator: () => { } },
    { field: 'normalizedCompanyName', headerName: '受付会社', sortable: true, comparator: () => { } },
    { field: 'normalizedStoreName', headerName: '受付店舗', sortable: true, comparator: () => { } },
    { field: 'picName', headerName: '受付担当者', sortable: true, comparator: () => { } },
    { field: 'remark', headerName: '備考', sortable: true, comparator: () => { } }
  ];

  fromDateFilter: NgbDate | null;
  toDateFilter: NgbDate | null;
  memberNoFilter: string;
  requestTypesFilter: RequestTypeDto[];
  requestTypeCodeFilter: number;
  companyNameFilter: string;
  storeNameFilter: string;
  deviceCodeFilter: string;
  createdByFilter: string;

  constructor (
    private requestsReceiptedsClient: RequestsReceiptedsClient,
    private cardClient: CardsClient,
    private authService: AuthorizeService,
    private calendar: NgbCalendar,
    public formatter: NgbDateParserFormatter,
    private common: CommonMethodsService,
    private translate: TranslateService,
    private route: ActivatedRoute,
  ) { }

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

  ngOnInit() {
    this.cardClient.getListClassifyRequest().subscribe(res => {
      var option = new RequestTypeDto();
      option.id = 0;
      this.requestTypes = res;
      this.requestTypeCode = null;

      this.route.queryParams.subscribe(params => {
        if (Object.keys(params).length) {
          if (params['memberNo']) {
            this.memberNo = params['memberNo'];
          }
          if (params['requestTypeCode']) {
            this.requestTypeCode = params['requestTypeCode'] == 'null' ? null : +params['requestTypeCode'];
          }
          if (params['companyName']) {
            this.companyName = params['companyName'];
          }
          if (params['storeName']) {
            this.storeName = params['storeName'];
          }
          if (params['deviceCode']) {
            this.deviceCode = params['deviceCode'];
          }
          if (params['fromDate']) {
            this.fromDate = this.parseDate(params['fromDate']);
            this.fromDateModel = params['fromDate'];
          }
          if (params['toDate']) {
            this.toDate = this.parseDate(params['toDate']);
            this.toDateModel = params['toDate'];
          }
          if (params['createdBy']) {
            this.createdBy = params['createdBy'];
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
          this.MapParamToSearch();
        } else {
          this.rowData = [];
          this.isInitGrid = true;
        }
      });
    });
  }

  onSortCommon(event) {
    if (this.totalRecords == 0) {
      this.commonGrid.clearSort();
      return;
    }

    if (this.rowData.length > 0) {
      this.orderBy = event.field;
      this.orderType = event.sort;
    }

    if (event.field == '' && this.rowData.length == 0) {
      return;
    }

    this.searchData();
  }

  onRowDoubleClickCommon(event) {
    this.authService.navigateToUrlWithoutReplace(`/history-inquiry/${event.id}`);
  }

  onRowClickCommon(event) {
  }

  onPageChangeEvent(event) {
    if (this.page != event) {
      this.page = event;
      this.searchData();
    }
  }

  onExportClickedEvent(event) {
    let fromDateSearch = this.fromDateFilter == null ? null : new Date(this.fromDateFilter.year, this.fromDateFilter.month - 1, this.fromDateFilter.day);
    let toDateSearch = this.toDateFilter == null ? null : new Date(this.toDateFilter.year, this.toDateFilter.month - 1, this.toDateFilter.day);
    this.requestsReceiptedsClient.exportRequestReceipt(this.memberNoFilter, this.requestTypeCodeFilter, this.companyNameFilter
      , this.storeNameFilter, this.deviceCodeFilter
      , fromDateSearch == null ? null : new Date(fromDateSearch.getTime() - (fromDateSearch.getTimezoneOffset() * 60000))
      , toDateSearch == null ? null : new Date(toDateSearch.getTime() - (toDateSearch.getTimezoneOffset() * 60000))
      , this.createdByFilter, false, null)
      .subscribe(result => {
        const blobContent = result.data;
        const fileSaveName = '申込履歴照会＿';
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
      }, err => {
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

  public handleSearchClick(): void {
    this.page = 1;
    this.pushSearchToParam();
  }

  pushSearchToParam() {
    const param = {
      memberNo: this.memberNo,
      requestTypeCode: this.requestTypeCode,
      companyName: this.companyName,
      storeName: this.storeName,
      deviceCode: this.deviceCode,
      createdBy: this.createdBy,
      fromDate: this.fromDate ? this.formatDate(this.fromDate) : '',
      toDate: this.toDate ? this.formatDate(this.toDate) : '',
      page: this.page,
      orderBy: this.orderBy,
      orderType: this.orderType
    };
    this.authService.navigateToUrlWithParam(`/history-inquiry`, param);
  }

  public searchData(): void {
    this.recordsPerPage = 10;
    let fromDateSearch = this.fromDateFilter == null ? null : new Date(this.fromDateFilter.year, this.fromDateFilter.month - 1, this.fromDateFilter.day);
    let toDateSearch = this.toDateFilter == null ? null : new Date(this.toDateFilter.year, this.toDateFilter.month - 1, this.toDateFilter.day);
    this.requestsReceiptedsClient.getRequestsReceiptedsWithPagination(this.page, this.recordsPerPage, this.memberNoFilter, this.requestTypeCodeFilter, this.companyNameFilter
      , this.storeNameFilter, this.deviceCodeFilter
      , fromDateSearch == null ? null : new Date(fromDateSearch.getTime() - (fromDateSearch.getTimezoneOffset() * 60000))
      , toDateSearch == null ? null : new Date(toDateSearch.getTime() - (toDateSearch.getTimezoneOffset() * 60000))
      , this.createdByFilter, this.orderBy, this.orderType, false, null)
      .subscribe(res => {
        this.rowData = res.items;
        this.totalRecords = res.totalCount;
        this.isShowButtonExport = res.items.length > 0;
        this.isInitGrid = false;
      });
  }

  public clearData(): void {
    this.authService.navigateToUrlWithParam(`/history-inquiry`, null);

    this.page = 1;
    this.memberNo = "";
    this.requestTypeCode = null;
    this.companyName = "";
    this.storeName = "";
    this.deviceCode = "";
    this.fromDate = null;
    this.toDate = null;
    this.createdBy = null;
    this.recordsPerPage = 0;
    this.rowData = [];
    this.totalRecords = 0;
    this.isShowButtonExport = false;
    this.isInitGrid = true;

    this.memberNoFilter = "";
    this.requestTypeCodeFilter = null;
    this.companyNameFilter = "";
    this.storeNameFilter = "";
    this.deviceCodeFilter = "";
    this.fromDateFilter = null;
    this.toDateFilter = null;
    this.createdByFilter = null;

    this.commonGrid.clearSort();
  }

  public MapParamToSearch(): void {
    this.memberNoFilter = this.memberNo;
    this.requestTypeCodeFilter = this.requestTypeCode;
    this.companyNameFilter = this.companyName;
    this.storeNameFilter = this.storeName;
    this.deviceCodeFilter = this.deviceCode;
    this.fromDateFilter = this.fromDate;
    this.toDateFilter = this.toDate;
    this.createdByFilter = this.createdBy;
    this.searchData();
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
