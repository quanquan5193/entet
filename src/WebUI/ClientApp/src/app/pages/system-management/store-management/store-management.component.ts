import { Component, OnInit, ViewChild } from '@angular/core';
import { NgModel } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { PermissionEnum } from 'src/app/share/constants/enum.constants';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';
import { ConfirmModalComponent } from 'src/app/share/modal/confirm-modal/confirm-modal.component';
import { NoticeModalComponent } from 'src/app/share/modal/notice-modal/notice-modal.component';
import { ApplicationUserDto, CompaniesClient, CreateStoreCommand, DeleteStoreCommand, FlatCompanyDto, StoresClient, UpdateStoreCommand } from 'src/app/web-api-client';
import { ConfirmCreateStoreModalComponent } from './confirm-create-store/confirm-create-store.component';

@Component({
  selector: 'app-store-management',
  templateUrl: './store-management.component.html',
  styleUrls: ['./store-management.component.scss'],
})
export class StoreManagementComponent implements OnInit {
  @ViewChild('commonGrid') commonGrid;
  @ViewChild('confirmModal') private confirmModal: ConfirmModalComponent;
  @ViewChild('noticeModal') private noticeModal: NoticeModalComponent;
  @ViewChild('closeModal') private closeModal: CloseModalComponent;
  @ViewChild('confirmCreateStoreModal') private confirmCreateStoreModalComponent: ConfirmCreateStoreModalComponent;
  @ViewChild('storeCodeValid') storeCodeValid: NgModel;
  @ViewChild('storeNameValid') storeNameValid: NgModel;
  @ViewChild('normalizedStoreNameValid') normalizedStoreNameValid: NgModel;
  @ViewChild('storeStatusValid') storeStatusValid: NgModel;
  @ViewChild('orderValid') orderValid: NgModel;

  companyIdSelected: any;
  companyCodeSelected: any;
  companyNameSelected: any;
  public storeCode: any;
  public storeName: any;
  public permission = PERMISSION;
  orderBy = 'Order';
  orderType = 'asc';

  companyCodeSelectedFilter: any;
  companyNameSelectedFilter: any;
  public storeCodeFilter: any;
  public storeNameFilter: any;

  storeIdSelected: any;
  storeCodeSelected: any;
  storeNameSelected: any;
  normalizedStoreNameSelected: any;
  displayFlagSelected: any;
  orderSelected: any;
  updateAtSelected: any;
  isActiveCreate: boolean;
  inValidStoreCode: boolean;
  checkValidateInput: boolean;
  listCompany: FlatCompanyDto[];

  storeStatus: any[];
  isInitGrid = true;
  rowData: any = [];
  page = 1;
  totalRecords = 0;
  recordsPerPage = 10;

  columnDefs = [
    { field: 'id', hide: true },
    { headerName: 'No.', minWidth: 70, maxWidth: 90, cellRenderer: node => this.getRowIndex(node.rowIndex) },
    { field: 'companyCode', headerName: '会社コード', sortable: true, comparator: () => { } },
    { field: 'storeCode', headerName: '店舗コード', sortable: true, comparator: () => { } },
    { field: 'storeName', headerName: '店舗名称', sortable: true, comparator: () => { } },
    { field: 'normalizedStoreName', headerName: '店舗略称', sortable: true, comparator: () => { } },
    {
      field: 'isActive', headerName: '表示フラグ', sortable: true, comparator: () => { }, valueFormatter: function (params) {
        var dateObj = params.value ? "表示" : "非表示";
        return dateObj;
      }
    },
    { field: 'order', headerName: '表示順', sortable: true, comparator: () => { } },
  ];

  constructor (
    private authService: AuthorizeService,
    private storesClient: StoresClient,
    private translate: TranslateService,
    private companiesClient: CompaniesClient
  ) { }

  ngOnInit() {
    this.storeStatus = [
      {
        "status": true,
        "name": "表示"
      },
      {
        "status": false,
        "name": "非表示"
      }];
    this.isActiveCreate = true;
    this.companiesClient.getCompanies().subscribe(res => {
      this.listCompany = res.lists;
    });
  }

  companyCodeFocusout() {
    const companyEnity = this.listCompany.filter(x => x.companyCode == this.companyCodeSelected);
    if (companyEnity != null && companyEnity.length == 1) {
      this.companyNameSelected = companyEnity[0].companyName;
    }
    else {
      this.companyNameSelected = null;
    }
  }

  getRowIndex(index) {
    return (index + 1) + ((this.page - 1) * this.recordsPerPage);
  }

  handleSearchData() {
    this.mapDataForSearch();
    this.searchData();
  }

  mapDataForSearch(): void {
    this.companyCodeSelectedFilter = this.companyCodeSelected;
    this.companyNameSelectedFilter = this.companyNameSelected;
    this.storeCodeFilter = this.storeCode;
    this.storeNameFilter = this.storeName;
  }

  searchData() {
    this.companyIdSelected = null;

    this.storesClient.getStoresWithPagination(this.companyCodeSelectedFilter, this.companyNameSelectedFilter, this.storeCodeFilter, this.storeNameFilter, this.orderBy, this.orderType, this.page, this.recordsPerPage)
      .subscribe(res => {
        this.rowData = res.items;
        this.totalRecords = res.totalCount;
        this.isInitGrid = false;

      });
  }

  validateModel(model) {
    let result = true;
    const codeRegex = /^([0-9]{4})?$/;
    const orderRegex = /^[0-9]*$/;
    if (!codeRegex.test(model.storeCode)) {
      this.storeCodeValid.control.markAsDirty();
      this.storeCodeValid.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.storeName) {
      this.storeNameValid.control.markAsDirty();
      this.storeNameValid.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.normalizedStoreName) {
      this.normalizedStoreNameValid.control.markAsDirty();
      this.normalizedStoreNameValid.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (model.isActive == undefined || model.isActive == null) {
      this.storeStatusValid.control.markAsDirty();
      this.storeStatusValid.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!orderRegex.test('' + model.order)) {
      this.orderValid.control.markAsDirty();
      this.orderValid.control.setErrors({ 'incorrect': true });
      result = false;
    }

    return result;
  }

  confirmCreateStore() {

    let createStoreCommand = new CreateStoreCommand();
    createStoreCommand.companyCode = this.companyCodeSelected;
    createStoreCommand.storeCode = this.storeCodeSelected;
    createStoreCommand.storeName = this.storeNameSelected;
    createStoreCommand.normalizedStoreName = this.normalizedStoreNameSelected;
    createStoreCommand.isActive = this.displayFlagSelected;
    createStoreCommand.order = this.orderSelected;

    if (!this.validateModel(createStoreCommand)) {
      this.noticeModal.open(this.translate.instant('systemManagement.common.errorInput'));
      return;
    }

    if (!this.companyCodeSelected) {
      this.noticeModal.open(this.translate.instant('systemManagement.store.companyNotFound'));
      return;
    }


    this.confirmCreateStoreModalComponent.open(createStoreCommand).then(res => {
      if (res) {
        this.createStore(createStoreCommand);
      }
    });
  }

  createStore(createStoreCommand: CreateStoreCommand) {
    this.storesClient.create(createStoreCommand).subscribe(res => {
      this.closeModal.open(this.translate.instant('systemManagement.store.createSuccessMessage')).then(() => {
        this.clearData();
        this.searchData();
      });
    }, async error => {
      if (error?.response && error?.response !== '') {
        const errorResponseJson = JSON.parse(error.response);
        if (errorResponseJson?.errors?.Order &&
          ((errorResponseJson?.errors?.StoreCode && errorResponseJson?.errors?.StoreCode[0] === "StoreCodeExisted") || (errorResponseJson?.errors[''] && errorResponseJson?.errors[''][0] === "StoreCodeExisted")) && errorResponseJson?.errors?.Order[0] === "OrderExisted") {
          this.noticeModal.open(this.translate.instant('systemManagement.store.existStoreCodeMessage') + '<br/>' + this.translate.instant('systemManagement.store.existStoreOrderMessage'));
          this.storeCodeValid.control.markAsDirty();
          this.storeCodeValid.control.setErrors({ 'incorrect': true });
          this.orderValid.control.markAsDirty();
          this.orderValid.control.setErrors({ 'incorrect': true });
        }
        else if ((errorResponseJson?.errors?.StoreCode && errorResponseJson?.errors?.StoreCode[0] === "StoreCodeExisted") || (errorResponseJson?.errors[''] && errorResponseJson?.errors[''][0] === "StoreCodeExisted")) {
          this.noticeModal.open(this.translate.instant('systemManagement.store.existStoreCodeMessage'));
          this.storeCodeValid.control.markAsDirty();
          this.storeCodeValid.control.setErrors({ 'incorrect': true });
        }
        else if (errorResponseJson?.errors?.Order && errorResponseJson?.errors?.Order[0] === "OrderExisted") {
          this.noticeModal.open(this.translate.instant('systemManagement.store.existStoreOrderMessage'));
          this.orderValid.control.markAsDirty();
          this.orderValid.control.setErrors({ 'incorrect': true });
        }
        else if (errorResponseJson?.errors?.CompanyId && errorResponseJson?.errors?.CompanyId[0] === "CompanyNotExisted") {
          this.noticeModal.open(this.translate.instant('systemManagement.store.existStoreIdMessage'));
        } else if (errorResponseJson.status == 400) {
          this.noticeModal.open(this.getErrorMessage(errorResponseJson));
        }
        else {
          this.noticeModal.open(this.translate.instant('systemManagement.store.companyNotFound'));
        }
      }
    });
  }

  confirmUpdateStore() {
    this.confirmModal.open(this.translate.instant('systemManagement.store.confirmUpdateMessage')).then(res => {
      if (res) {
        this.updateStore();
      }
    });
  }

  updateStore() {
    let updateStoreCommand = new UpdateStoreCommand();
    updateStoreCommand.id = this.storeIdSelected;
    updateStoreCommand.storeCode = this.storeCodeSelected;
    updateStoreCommand.storeName = this.storeNameSelected;
    updateStoreCommand.normalizedStoreName = this.normalizedStoreNameSelected;
    updateStoreCommand.isActive = this.displayFlagSelected;
    updateStoreCommand.order = this.orderSelected;
    updateStoreCommand.updatedAt = this.updateAtSelected;

    if (!this.validateModel(updateStoreCommand)) {
      this.noticeModal.open(this.translate.instant('systemManagement.common.errorInput'));
      return;
    }

    this.storesClient.update(this.storeIdSelected, updateStoreCommand).subscribe(res => {
      this.closeModal.open(this.translate.instant('systemManagement.store.updateSuccessMessage')).then(() => {
        this.clearData();
        this.searchData();
      });
    }, async error => {
      if (error?.response && error?.response !== '') {
        const errorResponseJson = JSON.parse(error.response);
        if (errorResponseJson?.title === "StoreCodeExisted") {
          this.noticeModal.open(this.translate.instant('systemManagement.store.existStoreCodeMessage'));
          this.storeCodeValid.control.markAsDirty();
          this.storeCodeValid.control.setErrors({ 'incorrect': true });
        }
        else if (errorResponseJson?.title === "OrderExisted") {
          this.noticeModal.open(this.translate.instant('systemManagement.store.existStoreOrderMessage'));
          this.orderValid.control.markAsDirty();
          this.orderValid.control.setErrors({ 'incorrect': true });
        }
        else if (errorResponseJson?.title === "DataChanged") {
          this.noticeModal.open(this.translate.instant('historyInquiry.dataChanged'));
          this.clearData();
        }
        else if (errorResponseJson?.title === "EntityDeleted") {
          this.noticeModal.open(this.translate.instant('systemManagement.store.entityDeleted'));
          this.clearData();
        }
        else if (errorResponseJson.status === 400) {
          this.noticeModal.open(this.getErrorMessage(errorResponseJson));
        }
      }
    });
  }

  getErrorMessage(errorModel): string {
    let errStr = '';
    for (var property in errorModel.errors) {
      if (errorModel.errors.hasOwnProperty(property)) {
        if (errStr !== '') {
          errStr = errStr + '<br/>';
        }
        errStr = errStr + this.translate.instant(`systemManagement.store.${errorModel.errors[property][0]}`);
      }
    }
    return errStr;
  }

  confirmDeleteStore() {
    this.confirmModal.open(this.translate.instant('systemManagement.store.confirmDeleteMessage'),
      this.translate.instant('systemManagement.store.confirmDeleteNotice')).then(res => {
        if (res) {
          this.deleteStore();
        }
      });
  }

  deleteStore() {
    let deleteStoreCommand = new DeleteStoreCommand();
    deleteStoreCommand.id = this.storeIdSelected;
    deleteStoreCommand.updatedAt = this.updateAtSelected;

    this.storesClient.delete(this.storeIdSelected, deleteStoreCommand).subscribe(res => {
      this.closeModal.open(this.translate.instant('systemManagement.store.deleteSuccessMessage')).then(() => {
        this.searchData();
        this.clearData();
      });
    }, error => {
      if (error?.response && error?.response !== '') {
        const errorResponseJson = JSON.parse(error.response);
        if (errorResponseJson?.title === "DataChanged") {
          this.noticeModal.open(this.translate.instant('systemManagement.store.dataChanged'));
          this.clearData();
        } else if (errorResponseJson?.title === "EntityDeleted") {
          this.noticeModal.open(this.translate.instant('systemManagement.store.entityDeletedDel'));
          this.clearData();
        }
        else if (errorResponseJson?.title === "StoreInUse") {
          this.noticeModal.open(this.translate.instant('systemManagement.store.storeInUse'));
          this.clearData();
        }
      }
    });
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

  }

  onRowClickCommon(event) {

    this.isActiveCreate = false;
    this.storeIdSelected = null;
    this.storeCodeSelected = null;
    this.storeNameSelected = null;
    this.normalizedStoreNameSelected = null;
    this.displayFlagSelected = null;
    this.orderSelected = null;
    this.updateAtSelected = null;

    this.storeIdSelected = event.id;
    this.storeCodeSelected = event.storeCode;
    this.storeNameSelected = event.storeName;
    this.normalizedStoreNameSelected = event.normalizedStoreName;
    this.displayFlagSelected = event.isActive;
    this.orderSelected = event.order;
    this.updateAtSelected = event?.updatedAt ? new Date(new Date(event?.updatedAt).getTime() - (new Date(event?.updatedAt).getTimezoneOffset() * 60000)) : null;
  }

  onPageChangeEvent(event) {
    this.page = event;
    this.searchData();
  }

  clearData() {
    this.companyIdSelected = null;
    this.checkValidateInput = false;
    this.isActiveCreate = true;
    this.storeIdSelected = null;
    this.storeCodeSelected = null;
    this.storeNameSelected = null;
    this.normalizedStoreNameSelected = null;
    this.displayFlagSelected = null;
    this.orderSelected = null;
    this.updateAtSelected = null;
    this.setInputsPristine();
  }

  validateOrder(event: number) {
    if (event < 1) {
      this.orderSelected = null;
    }
  }

  inputControl = {};

  handleInputFocus(event, key: string) {
    this.inputControl[key] = event;
  }
  handleInputStoreNameFocusOut(data) {    
    this.storeNameSelected = data.trim();
  }
  handleInputStoreNameNormalFocusOut(data) {
    this.normalizedStoreNameSelected = data.trim();
  }
  setInputsPristine() {
    for (var property in this.inputControl) {
      if (this.inputControl.hasOwnProperty(property)) {
        this.inputControl[property].control.markAsPristine();
      }
    }
  }

  setInputPristine(element: NgModel) {
    element.control.markAsPristine();
  }
}
