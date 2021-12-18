import { Component, OnInit, ViewChild, forwardRef } from '@angular/core';
import { NgbCalendar, NgbDate, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import * as dayjs from 'dayjs';
import { CONFIG } from 'src/app/share/constants/config.constants';
import { DATA } from 'src/app/share/constants/data.constants';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';
import { ConfirmModalComponent } from 'src/app/share/modal/confirm-modal/confirm-modal.component';
import { NoticeModalComponent } from 'src/app/share/modal/notice-modal/notice-modal.component';
import { CardDto, DeleteCardsCommand, DeleteKidsCommand, KidsClubClient, MemberKidDeleteCommandDto, MemberKidDeleteDto } from 'src/app/web-api-client';

@Component({
  selector: 'app-delete-kid-club',
  templateUrl: './delete-kid-club.component.html',
  styleUrls: ['./delete-kid-club.component.scss'],
})
export class DeleteKidClubComponent implements OnInit {
  @ViewChild('commonGrid') commonGrid;
  @ViewChild("noticeModal") private noticeModalComponent: NoticeModalComponent;
  @ViewChild("confirmModal") private confirmModalComponent: ConfirmModalComponent;
  @ViewChild('closeModal') private closeModalComponent: CloseModalComponent;
  hoveredDate: NgbDate | null = null;
  listStatus = DATA.CardStatusDropdown;
  public info = '';
  rowData: any;
  isInitGrid = true;
  columnDefs = [
    { headerName: 'No.', minWidth: 70, maxWidth: 90, cellRenderer: node => this.getRowIndex(node.rowIndex) },
    { field: 'createdAt', headerName: '受付日', minWidth: 130, sortable: true, comparator: () => { }, cellRenderer: date => dayjs(date.value).format(CONFIG.DateFormat) },
    { field: 'memberNo', headerName: 'お客様番号', minWidth: 140, sortable: true, comparator: () => { } },
    { field: 'parentName', headerName: '保護者氏名', minWidth: 120, sortable: true, comparator: () => { } },
    { field: 'kidName', headerName: '子名前', minWidth: 120, sortable: true, comparator: () => { } },
    { field: 'sex', headerName: '性別', minWidth: 50, maxWidth: 250, sortable: true, comparator: () => { }, cellRenderer: sex => this.handleSex(sex.value) },
    { field: 'dateOfBirth', headerName: '生年月日', minWidth: 50, maxWidth: 250, sortable: true, comparator: () => { }, cellRenderer: date => dayjs(date.value).format(CONFIG.DateFormat) },
    { field: 'relationshipMember', headerName: '続柄', minWidth: 50, maxWidth: 250, sortable: true, comparator: () => { }, cellRenderer: relationshipMember => this.handleKidRelationship(relationshipMember.value) },
    { field: 'company.companyCode', headerName: '会社コード', minWidth: 50, sortable: true, comparator: () => { } },
    { field: 'store.storeCode', headerName: '店舗コード', minWidth: 50, sortable: true, comparator: () => { } }
  ];

  isShowButtonExport = false;
  page = 1;
  totalRecords = 0;
  recordsPerPage = 10;
  pageLoad = 1;
  recordsPerLoad = 70000;
  sortBy = '';
  sortType = 'asc';
  companyCode = '';
  storeCode = '';
  fromDate: NgbDate | null;
  toDate: NgbDate | null;
  fromDateModel: any;
  toDateModel: any;
  deviceCode = '';
  status = null;
  expirationDate = '';

  listCardsDto: CardDto[];
  listCardsPaginationDto: CardDto[];

  constructor (
    private calendar: NgbCalendar,
    public formatter: NgbDateParserFormatter,
    private kidClients: KidsClubClient,
    private translate: TranslateService
  ) {

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

  getRowIndex(index) {
    return (index + 1) + ((this.page - 1) * this.recordsPerPage);
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
    this.rowData = [];
  }

  handleSearch() {
    this.page = 1;
    this.load();
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

  handleClearData() {
    this.rowData = [];
    this.totalRecords = 0;
    this.sortBy = '';
    this.sortType = 'asc';
    this.deviceCode = '';
    this.expirationDate = '';
    this.companyCode = '';
    this.storeCode = '';
    this.status = null;
    this.fromDate = null;
    this.toDate = null;
    this.listCardsDto = [];
    this.listCardsPaginationDto = [];
    this.isInitGrid = true;
    this.commonGrid.clearSort();
  }

  handleClickDelete() {
    if (this.listCardsDto.length > this.recordsPerLoad) {
      this.noticeModalComponent.open(this.translate.instant('batchProcess.deleteMoreThan70000'));
      return;
    }

    const deleteTitle = this.translate.instant('batchProcess.confirmKidDelete');
    const noticeDeleteTitle = this.translate.instant('batchProcess.noticeKidDelete');
    this.confirmModalComponent.open(deleteTitle, noticeDeleteTitle).then(res => {
      if (res) this.deleteKids();
    });
  }

  async deleteKids() {
    let param: DeleteKidsCommand = new DeleteKidsCommand();
    param.kidsDelete = [];
    this.listCardsDto.forEach(kid => {
      let dto: MemberKidDeleteCommandDto = new MemberKidDeleteCommandDto();
      dto.id = kid.id;
      if (kid.updatedAt) dto.updatedAt = this.handleQueryDate(kid.updatedAt);
      param.kidsDelete.push(dto);
    });
    this.kidClients.deleteMemberKids(param).subscribe(res => {
      if (res) {
        this.closeModalComponent.open(this.translate.instant('batchProcess.deleteKidSuccess')).then(() => this.load());
      }
    }, error => {
      const errorResponse = error.response;
      const errorResponseJson = JSON.parse(errorResponse);
      if (errorResponseJson.title === "EntityDeleted") {
        this.noticeModalComponent.open(this.translate.instant('batchProcess.entityDeleted'));
      } else if (errorResponseJson.title === "DataChanged") {
        this.noticeModalComponent.open(this.translate.instant('batchProcess.dataChanged'));
      } else this.noticeModalComponent.open(this.translate.instant('system.error.unknownException'));
    });
  }

  load() {
    this.kidClients.getKidsDeleteWithPagination(
      this.pageLoad,
      this.recordsPerLoad,
      this.sortType,
      this.sortBy,
      this.companyCode,
      this.storeCode,
      this.deviceCode,
      this.formatDate(this.fromDate),
      this.formatDate(this.toDate)
    )
      .subscribe(result => {
        this.totalRecords = result.totalCount;
        this.listCardsPaginationDto = result.items;
        this.listCardsDto = result.items;
        this.filterDataToGrid(this.page);
        this.isInitGrid = false;
      }, (error) => {
        const errorResponse = error.response;
        const errorResponseJson = JSON.parse(errorResponse);
        if (errorResponseJson.status === 400 && errorResponseJson.errors['MoreThanRecord']) {
          this.noticeModalComponent.open(this.translate.instant('batchProcess.deleteMoreThanFirst') + errorResponseJson.errors['MoreThanRecord'][0] + this.translate.instant('batchProcess.deleteMoreThanSecond'));
        } else {
          this.closeModalComponent.open(this.translate.instant('system.error.fetchDataFail'));
        }
      });
  }

  filterDataToGrid(page) {
    this.rowData = this.listCardsPaginationDto.slice((page - 1) * this.recordsPerPage, ((page - 1) * this.recordsPerPage) + this.recordsPerPage);
  }

  handleQueryDate(date: Date) {
    return new Date(date.getTime() - (date.getTimezoneOffset() * 60000));
  }

  formatDate(date: NgbDateStruct): string {
    if (date) return '' + date.year + '/' + (date.month <= 9 ? '0' + date.month : date.month) + '/' + (date.day <= 9 ? '0' + date.day : date.day);
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

  onRowDoubleClickCommon(event) { }

  onRowClickCommon(event) { }

  onPageChangeEvent(event) {
    this.page = event;
    this.filterDataToGrid(this.page);
  }

  onExportClickedEvent(event) { }

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
