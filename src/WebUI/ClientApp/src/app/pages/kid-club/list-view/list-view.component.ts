import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbCalendar, NgbDate, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import * as dayjs from 'dayjs';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { CONFIG } from 'src/app/share/constants/config.constants';
import { DATA } from 'src/app/share/constants/data.constants';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';
import { CommonMethodsService } from 'src/app/share/services/common-methods.service';
import { KidsClubClient } from 'src/app/web-api-client';
import { Location } from "@angular/common";

@Component({
  selector: 'app-list-view-kid-club-table',
  templateUrl: './list-view.component.html',
  styleUrls: ['./list-view.component.scss'],
})
export class ListViewKidClubComponent implements OnInit {
  @ViewChild('commonGrid') commonGrid;
  @ViewChild('closeModal') private closeModalComponent: CloseModalComponent;
  public permission = PERMISSION;
  columnDefs = [
    { headerName: 'No.', minWidth: 70, maxWidth: 90, cellRenderer: node => this.getRowIndex(node.rowIndex) },
    { field: 'createdAt', headerName: '受付日', minWidth: 50, maxWidth: 250, sortable: true, comparator: () => { }, cellRenderer: date => dayjs(date.value).format(CONFIG.DateFormat) },
    { field: 'memberNo', headerName: 'お客様番号', minWidth: 50, maxWidth: 250, sortable: true, comparator: () => { } },
    { field: 'parentLastName', headerName: '保護者氏名', minWidth: 50, maxWidth: 250, sortable: true, comparator: () => { }, cellRenderer: event => event.data.parentName },
    { field: 'lastName', headerName: 'お子様氏名', minWidth: 50, maxWidth: 250, sortable: true, comparator: () => { }, cellRenderer: event => event.data.fullName },
    { field: 'sex', headerName: '性別', minWidth: 50, maxWidth: 250, sortable: true, comparator: () => { }, cellRenderer: sex => this.handleSex(sex.value) },
    { field: 'dateOfBirth', headerName: '生年月日', minWidth: 50, maxWidth: 250, sortable: true, comparator: () => { }, cellRenderer: date => dayjs(date.value).format(CONFIG.DateFormat) },
    { field: 'relationshipMember', headerName: '続柄', minWidth: 50, maxWidth: 250, sortable: true, comparator: () => { }, cellRenderer: relationshipMember => this.handleKidRelationship(relationshipMember.value) },
    { field: 'picStoreName', headerName: '受付担当者', minWidth: 50, maxWidth: 250, sortable: true, comparator: () => { } }
  ];

  listStatus = DATA.CardStatusDropdown;
  hoveredDate: NgbDate | null = null;
  rowData = [];
  isShowButtonExport = false;
  page = 1;
  totalRecords: number;
  recordsPerPage = 10;
  memberNo = "";
  kidName = "";
  companyCode = "";
  storeCode = "";
  deviceNumber = "";
  pICStoreName = "";
  size = 10;
  sortBy = "";
  sortDimension = "";
  fromDate: NgbDate | null;
  toDate: NgbDate | null;
  fromDateModel: any;
  toDateModel: any;
  isInitGrid = true;

  memberNoFilter = "";
  kidNameFilter = "";
  companyCodeFilter = "";
  storeCodeFilter = "";
  deviceNumberFilter = "";
  pICStoreNameFilter = "";
  fromDateFilter: NgbDate | null;
  toDateFilter: NgbDate | null;

  constructor (
    private authService: AuthorizeService,
    private calendar: NgbCalendar,
    private kidsClubClients: KidsClubClient,
    public formatter: NgbDateParserFormatter,
    private common: CommonMethodsService,
    private route: ActivatedRoute,
    private translate: TranslateService,
    private location: Location
  ) {
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (Object.keys(params).length) {
        if (params['memberNo']) {
          this.memberNo = params['memberNo'];
        }
        if (params['kidName']) {
          this.kidName = params['kidName'];
        }
        if (params['companyCode']) {
          this.companyCode = params['companyCode'];
        }
        if (params['storeCode']) {
          this.storeCode = params['storeCode'];
        }
        if (params['deviceNumber']) {
          this.deviceNumber = params['deviceNumber'];
        }
        if (params['fromDate']) {
          this.fromDate = this.parseDate(params['fromDate']);
          this.fromDateModel = params['fromDate'];
        }
        if (params['toDate']) {
          this.toDate = this.parseDate(params['toDate']);
          this.toDateModel = params['toDate'];
        }
        if (params['pICStoreName']) {
          this.pICStoreName = params['pICStoreName'];
        }
        if (params['page']) {
          this.page = params['page'];
        }
        if (params['sortBy']) {
          this.sortBy = params['sortBy'];
        }
        if (params['sortDimension']) {
          this.sortDimension = params['sortDimension'];
        }
        this.mapDataForSearch();
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
      kidName: this.kidName,
      companyCode: this.companyCode,
      storeCode: this.storeCode,
      deviceNumber: this.deviceNumber,
      fromDate: this.fromDate ? this.formatDate(this.fromDate) : '',
      toDate: this.toDate ? this.formatDate(this.toDate) : '',
      pICStoreName: this.pICStoreName,
      page: this.page,
      sortBy: this.sortBy,
      sortDimension: this.sortDimension
    };
    this.authService.navigateToUrlWithParam(`/kid-club`, param);
  }

  mapDataForSearch() {
    this.memberNoFilter = this.memberNo;
    this.kidNameFilter = this.kidName;
    this.companyCodeFilter = this.companyCode;
    this.storeCodeFilter = this.storeCode;
    this.deviceNumberFilter = this.deviceNumber;
    this.pICStoreNameFilter = this.pICStoreName;
    this.fromDateFilter = this.fromDate;
    this.toDateFilter = this.toDate;
    this.load();
  }

  handleClearData() {
    this.authService.navigateToUrlWithParam(`/kid-club`, null);
    this.rowData = [];
    this.totalRecords = 0;
    this.sortBy = 'Id';
    this.sortDimension = 'asc';
    this.memberNo = "";
    this.kidName = "";
    this.companyCode = "";
    this.storeCode = "";
    this.deviceNumber = "";
    this.pICStoreName = "";
    this.size = 10;
    this.sortBy = "";
    this.sortDimension = "";
    this.fromDate = null;
    this.toDate = null;
    this.isShowButtonExport = false;
    this.isInitGrid = true;

    this.memberNoFilter = "";
    this.kidNameFilter = "";
    this.companyCodeFilter = "";
    this.storeCodeFilter = "";
    this.deviceNumberFilter = "";
    this.pICStoreNameFilter = "";
    this.fromDateFilter = null;
    this.toDateFilter = null;

    this.commonGrid.clearSort();
  }

  getRowIndex(index) {
    return (index + 1) + ((this.page - 1) * this.recordsPerPage);
  }

  handleQueryDate(date: Date) {
    return new Date(date.getTime() - (date.getTimezoneOffset() * 60000));
  }

  load() {
    this.kidsClubClients.getPaging(
      this.memberNoFilter,
      this.kidNameFilter,
      this.companyCodeFilter,
      this.storeCodeFilter,
      this.deviceNumberFilter,
      this.fromDateFilter == null ? null : this.handleQueryDate(new Date(this.fromDateFilter.year, this.fromDateFilter.month - 1, this.fromDateFilter.day)),
      this.toDateFilter == null ? null : this.handleQueryDate(new Date(this.toDateFilter.year, this.toDateFilter.month - 1, this.toDateFilter.day)),
      this.pICStoreNameFilter,
      this.page,
      this.size,
      this.sortBy,
      this.sortDimension
    ).subscribe(result => {
      this.totalRecords = result.totalCount;
      this.rowData = result.items;
      this.isShowButtonExport = this.rowData.length > 0;
      this.isInitGrid = false;
    });
  }

  handleSex(status) {
    switch (status) {
      case DATA.SexType.Male:
        return '男';
      case DATA.SexType.Female:
        return '女';
      case DATA.SexType.Other:
        return 'その他';
    }
  }

  handleKidRelationship(status) {
    switch (status) {
      case DATA.KidRelationship.Unset:
        return '';
      case DATA.KidRelationship.Father:
        return '父';
      case DATA.KidRelationship.Mother:
        return '母';
      case DATA.KidRelationship.GrandFarther:
        return '祖父';
      case DATA.KidRelationship.GrandMother:
        return '祖母';
      case DATA.KidRelationship.Other:
        return 'その他';
    }
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

  isRange(date: NgbDate) {
    return date.equals(this.fromDate) || (this.toDate && date.equals(this.toDate)) || this.isInside(date) || this.isHovered(date);
  }

  isInside(date: NgbDate) {
    return this.toDate && date.after(this.fromDate) && date.before(this.toDate);
  }

  isHovered(date: NgbDate) {
    return this.fromDate && !this.toDate && this.hoveredDate && date.after(this.fromDate) && date.before(this.hoveredDate);
  }

  onSortCommon(event) {
    if (this.totalRecords == 0 || this.totalRecords == undefined) {
      this.commonGrid.clearSort();
      return;
    }

    if (event.field !== null || event.field !== undefined) {
      this.sortBy = event.field;
      this.sortDimension = event.sort;
    }

    if (event.field == '' && this.rowData.length == 0) {
      return;
    }

    this.load();
  }

  onRowDoubleClickCommon(event) {
    this.authService.navigateToUrlWithoutReplace(`/kid-club/${event.id}`);
  }

  onPageChangeEvent(event) {
    if (this.page != event) {
      this.page = event;
      this.load();
    }
  }

  validateInput(currentValue: NgbDate | null, input: string): NgbDate | null {
    const parsed = this.formatter.parse(input);
    return parsed && this.calendar.isValid(NgbDate.from(parsed)) ? NgbDate.from(parsed) : currentValue;
  }

  onRowClickCommon(event) {
  }

  onExportClickedEvent(event) {
    this.kidsClubClients.exportKids(
      this.memberNoFilter,
      this.kidNameFilter,
      this.companyCodeFilter,
      this.storeCodeFilter,
      this.deviceNumberFilter,
      this.fromDateFilter == null ? null : this.handleQueryDate(new Date(this.fromDateFilter.year, this.fromDateFilter.month - 1, this.fromDate.day)),
      this.toDateFilter == null ? null : this.handleQueryDate(new Date(this.toDateFilter.year, this.toDateFilter.month - 1, this.toDateFilter.day)),
      this.pICStoreNameFilter,
      this.sortBy,
      this.sortDimension
    ).subscribe(result => {
      const blobContent = result.data;
      const fileSaveName = 'キッズクラブ登録情報＿';
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
