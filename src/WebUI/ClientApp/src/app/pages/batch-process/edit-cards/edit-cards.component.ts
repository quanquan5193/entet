import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbCalendar, NgbDate, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import * as dayjs from 'dayjs';
import { DATA } from 'src/app/share/constants/data.constants';
import { CONFIG } from 'src/app/share/constants/config.constants';
import { FlatCompanyDto, FlatStoreDto, CardsClient, CompaniesClient, StoresClient, CompaniesVm, StoresVm, CardDto, UpdateCardsCommand } from 'src/app/web-api-client';
import { NoticeModalComponent } from 'src/app/share/modal/notice-modal/notice-modal.component';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmModalComponent } from 'src/app/share/modal/confirm-modal/confirm-modal.component';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';
import { isNumber, padNumber } from 'src/app/share/factories/ng-bootstrap-date-formatter.factory';
@Component({
  selector: 'app-edit-cards',
  templateUrl: './edit-cards.component.html',
  styleUrls: ['./edit-cards.component.scss'],
})
export class EditCardsComponent implements OnInit {
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
    { field: 'memberNo', headerName: 'お客様番号', minWidth: 140, sortable: true, comparator: () => { } },
    { field: 'createdAt', headerName: '登録日', minWidth: 130, sortable: true, comparator: () => { }, cellRenderer: date => dayjs(date.value).format(CONFIG.DateFormat) },
    { field: 'expiredAt', headerName: '有効期限', minWidth: 120, sortable: true, comparator: () => { }, cellRenderer: dateExp => dayjs(dateExp.value).format(CONFIG.DateShortFormat) },
    { field: 'status', headerName: '状態', sortable: true, comparator: () => { }, cellRenderer: status => this.handleFilterStatus(status.value) },
    { field: 'company.companyCode', headerName: '会社コード', minWidth: 50, sortable: true, comparator: () => { } },
    { field: 'store.storeCode', headerName: '店舗コード', minWidth: 50, sortable: true, comparator: () => { } },
    { field: 'createdByName', headerName: '登録者', minWidth: 50, sortable: true, comparator: () => { } },
    { field: 'updatedAt', headerName: '更新日', minWidth: 130, sortable: true, comparator: () => { }, cellRenderer: date => date.value ? dayjs(date.value).format(CONFIG.DateFormat) : '' },
    { field: 'updatedByName', headerName: '更新者', minWidth: 50, sortable: true, comparator: () => { } }
  ];

  isShowButtonExport = false;
  page = 1;
  totalRecords = 0;
  recordsPerPage = 10;
  pageLoad = 1;
  recordsPerLoad = 70000;
  sortBy = '';
  sortType = 'asc';
  memberNoFrom = '';
  memberNoTo = '';
  companyCode = '';
  storeCode = '';
  fromDate: NgbDate | null;
  toDate: NgbDate | null;
  fromDateModel: any;
  toDateModel: any;
  registeredBy = '';
  status = null;
  expirationDate = '';

  statusItemEdit: any;
  storeIdEdit: any;
  companyIdEdit: any;

  listCompanies: FlatCompanyDto[];
  listStores: FlatStoreDto[];
  listStoresAll: FlatStoreDto[];
  listCardsDto: CardDto[];
  listCardsPaginationDto: CardDto[];

  selectedCompany: FlatCompanyDto;
  selectedStore: FlatStoreDto;

  constructor (
    private authService: AuthorizeService,
    private calendar: NgbCalendar,
    public formatter: NgbDateParserFormatter,
    private cardClients: CardsClient,
    private companiesClient: CompaniesClient,
    private translate: TranslateService,
    private storesClient: StoresClient
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
    this.companiesClient.getCompanies().subscribe((data: CompaniesVm) => {
      this.listCompanies = data.lists;
    });
    this.storesClient.getStores().subscribe((data: StoresVm) => {
      this.listStoresAll = data.lists;
      this.listStores = data.lists;
    });
  }

  handleSearch() {
    if ((this.memberNoFrom && !this.memberNoTo) || (!this.memberNoFrom && this.memberNoTo)) {
      this.rowData = [];
      this.isInitGrid = false;
      return;
    }
    this.page = 1;
    this.load();
  }

  handleClearData() {
    this.rowData = [];
    this.totalRecords = 0;
    this.sortBy = '';
    this.sortType = 'asc';
    this.memberNoFrom = '';
    this.memberNoTo = '';
    this.expirationDate = '';
    this.companyCode = '';
    this.storeCode = '';
    this.status = null;
    this.fromDate = null;
    this.toDate = null;
    this.registeredBy = '';
    this.listCardsDto = [];
    this.listCardsPaginationDto = [];
    this.isInitGrid = true;
    this.commonGrid.clearSort();
  }

  handleClickUpdate() {
    if (this.listCardsDto.length > this.recordsPerLoad) {
      this.noticeModalComponent.open(this.translate.instant('batchProcess.updateMoreThan70000'));
      return;
    }

    const editTitle = this.translate.instant('batchProcess.confirmEdit');
    const noticeEditTitle = this.translate.instant('batchProcess.noticeEdit');
    let option = [];
    if (this.statusItemEdit) {
      option.push({
        Title: this.translate.instant('batchProcess.search.cardStatus'),
        Content: this.handleFilterStatus(+this.statusItemEdit)
      });
    }
    if (this.companyIdEdit) {
      option.push({
        Title: this.translate.instant('batchProcess.search.companyCode'),
        Content: `${this.selectedCompany.companyCode}  ${this.selectedCompany.companyName}`
      });
    }
    if (this.storeIdEdit) {
      option.push({
        Title: this.translate.instant('batchProcess.search.storeCode'),
        Content: `${this.selectedStore.storeCode}  ${this.selectedStore.storeName}`
      });
    }
    this.confirmModalComponent.open(editTitle, noticeEditTitle, option).then(res => {
      if (res) this.updateCards();
    });
  }

  async updateCards() {
    if (this.listCardsDto.length > 70000) {
      this.noticeModalComponent.open(this.translate.instant('batchProcess.updateMoreThan70000'));
      return;
    }

    if (this.companyIdEdit) {
      this.listCardsDto.forEach(card => {
        card.companyId = this.companyIdEdit;
      });
    }
    if (this.storeIdEdit) {
      this.listCardsDto.forEach(card => {
        card.storeId = this.storeIdEdit;
      });
    }
    if (this.statusItemEdit) {
      this.listCardsDto.forEach(card => {
        card.status = this.statusItemEdit;
      });
    }
    this.listCardsDto.forEach(card => {
      if (card.updatedAt) card.updatedAt = this.handleQueryDate(card.updatedAt);
    });
    let param: UpdateCardsCommand = new UpdateCardsCommand();
    param.cardsUpdate = this.listCardsDto;
    this.cardClients.editCards(param).subscribe(res => {
      if (res) {
        this.closeModalComponent.open(this.translate.instant('batchProcess.updateSuccess')).then(() => this.load());
      }
    }, error => {
      const errorResponse = error.response;
      const errorResponseJson = JSON.parse(errorResponse);
      if (errorResponseJson.title === "DataChanged") {
        this.noticeModalComponent.open(this.translate.instant('batchProcess.dataChanged'));
        this.load();
      } else if (errorResponseJson.title === "EntityDeleted") {
        this.noticeModalComponent.open(this.translate.instant('batchProcess.updateEntityDeleted'));
        this.load();
      } else if (errorResponseJson.status === 400) {
        this.noticeModalComponent.open(this.getErrorMessage(errorResponseJson));
        this.load();
      } else this.noticeModalComponent.open(this.translate.instant('system.error.unknownException'));
    });
  }

  getErrorMessage(errorModel): string {
    let errStr = '';
    for (var property in errorModel.errors) {
      if (errorModel.errors.hasOwnProperty(property)) {
        if (errStr !== '') {
          errStr = errStr + '<br/>';
        }
        errStr = errStr + this.translate.instant(`batchProcess.${errorModel.errors[property][0]}`);
      }
    }
    return errStr;
  }

  handleQueryDate(date: Date) {
    return new Date(date.getTime() - (date.getTimezoneOffset() * 60000));
  }

  load() {
    this.cardClients.getCardsEditWithPagination(
      this.sortBy,
      this.sortType,
      this.pageLoad.toString(),
      this.recordsPerLoad.toString(),
      this.memberNoFrom,
      this.memberNoTo,
      this.companyCode,
      this.storeCode,
      this.formatDate(this.fromDate),
      this.formatDate(this.toDate),
      this.registeredBy,
      this.status,
      this.expirationDate)
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
          this.noticeModalComponent.open(this.translate.instant('batchProcess.updateMoreThanFirst') + errorResponseJson.errors['MoreThanRecord'][0] + this.translate.instant('batchProcess.updateMoreThanSecond'));
        } else {
          this.closeModalComponent.open(this.translate.instant('system.error.fetchDataFail'));
        }
      });
  }

  filterDataToGrid(page) {
    this.rowData = this.listCardsPaginationDto.slice((page - 1) * this.recordsPerPage, ((page - 1) * this.recordsPerPage) + this.recordsPerPage);
  }

  companiesChanged(event: FlatCompanyDto) {
    if (event && event.id != this.selectedStore?.companyId) {
      this.storeIdEdit = null;
      this.selectedStore = null;
    }
    this.selectedCompany = event;
    this.listStores = this.filterListStore(event.id);
  }

  storesChanged(event: FlatStoreDto) {
    this.selectedStore = event;
  }

  filterListStore(id: number) {
    return this.listStoresAll.filter(n => n.companyId == id);
  }

  handleClearCompany() {
    this.storeIdEdit = null;
    this.selectedCompany = null;
    this.selectedStore = null;
    this.listStores = this.listStoresAll;
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
}
