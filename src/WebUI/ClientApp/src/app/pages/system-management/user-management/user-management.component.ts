import { Component, OnInit, ViewChild } from '@angular/core';
import { NgModel } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import * as dayjs from 'dayjs';
import { map } from 'rxjs/operators';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { CONFIG } from 'src/app/share/constants/config.constants';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { ConfirmModalComponent } from 'src/app/share/modal/confirm-modal/confirm-modal.component';
import { NoticeModalComponent } from 'src/app/share/modal/notice-modal/notice-modal.component';
import { CompaniesClient, CompaniesVm, CreateUserCommand, FlatCompanyDto, FlatStoreDto, StoresClient, StoresVm, UpdateUserCommand, UsersClient } from 'src/app/web-api-client';
import { ConfirmCreateModalComponent } from '../device-management/create/confirm-modal/confirm-create-modal.component';
import { ConfirmCreateUserModalComponent } from './comfirm-create-user/confirm-create-user.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss'],
})
export class UserManagementComponent implements OnInit {
  @ViewChild('commonGrid') commonGrid;
  @ViewChild('storeCode') storeCodeSelectBox;
  @ViewChild("confirmModal") private confirmModalComponent: ConfirmModalComponent;
  @ViewChild("noticeModal") private noticeModalComponent: NoticeModalComponent;
  @ViewChild("confirmCreateUserModal") private confirmCreateUserModal: ConfirmCreateUserModalComponent;
  @ViewChild("companyCode") companyCode: NgModel;
  @ViewChild("storeCode") storeCode: NgModel;
  @ViewChild("permission") permission: NgModel;
  @ViewChild("loginid") loginid: NgModel;
  @ViewChild("crtUserPwd") crtUserPwd: NgModel;
  @ViewChild("crtUserConfirmPwd") crtUserConfirmPwd: NgModel;
  @ViewChild("fullName") fullName: NgModel;

  public permissionRole = PERMISSION;
  isInitGrid = true;

  rowData: any = [];
  orderBy = 'Id';
  orderType = 'desc';
  isShowButtonExport = false;
  columnDefs = [
    { headerName: 'No.', minWidth: 70, maxWidth: 90, cellRenderer: node => this.getRowIndex(node.rowIndex) },
    { field: 'userName', headerName: 'ログインID', minWidth: 50, maxWidth: 300, sortable: true, comparator: () => { } },
    { field: 'permission', headerName: '権限', minWidth: 50, maxWidth: 300, sortable: true, comparator: () => { } },
    { field: 'normalizedCompanyName', headerName: '会社名称', minWidth: 50, maxWidth: 300, sortable: true, comparator: () => { } },
    { field: 'normalizedStoreName', headerName: '店舗名称', minWidth: 50, maxWidth: 300, sortable: true, comparator: () => { } },
    { field: 'fullName', headerName: '氏名', minWidth: 50, maxWidth: 300, sortable: true, comparator: () => { } },
    { field: 'createdAt', headerName: '登録日', minWidth: 50, maxWidth: 300, sortable: true, comparator: () => { }, cellRenderer: date => dayjs(date.value).format(CONFIG.DateFormat) },
  ];

  listCompanies: FlatCompanyDto[];
  listStores: FlatStoreDto[];
  listStoresAll: FlatStoreDto[];
  listRoles = [
    { id: '1', name: '1' },
    { id: '2', name: '2' },
    { id: '3', name: '3' },
    { id: '6', name: '6' },
    { id: '7', name: '7' },
    { id: '8', name: '8' },
    { id: '9', name: '9' },
    { id: '10', name: '10' },
  ];

  selectedItem: any = {};
  userPwd = '';
  userConfirmPwd = '';
  selectedCompany = null;
  selectedStore = null;

  //Search params
  companyCodeSearch: string;
  companyNameSearch: string;
  userNameSearch: string;
  fullNameSearch: string;

  companyCodeSearchFilter: string;
  companyNameSearchFilter: string;
  userNameSearchFilter: string;
  fullNameSearchFilter: string;

  page = 1;
  size = 10;
  sortBy: string;
  sortDimension: string;
  totalRecords = 0;
  recordsPerPage = 0;
  //End search params

  pwdRegex = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{1,}$/;
  userRegex = /^([0-9a-zA-Z]){6,18}$/;
  submitted = false;

  constructor (
    private companiesClient: CompaniesClient,
    private translate: TranslateService,
    private storesClient: StoresClient,
    private usersClients: UsersClient,
    private authService: AuthorizeService,
  ) { }

  ngOnInit() {
    this.rowData = [];
    this.companiesClient.getCompanies().subscribe((data: CompaniesVm) => {
      this.listCompanies = data.lists;
    });
    this.storesClient.getStores().subscribe((data: StoresVm) => {
      this.listStoresAll = data.lists;
      this.listStores = [];
    });
  }

  handleSearch() {
    this.page = 1;
    this.mapDataForSearch();
    this.load();
  }

  mapDataForSearch(): void {
    this.companyCodeSearchFilter = this.companyCodeSearch;
    this.companyNameSearchFilter = this.companyNameSearch;
    this.userNameSearchFilter = this.userNameSearch;
    this.fullNameSearchFilter = this.fullNameSearch;
  }

  load() {
    this.usersClients.get(this.companyCodeSearchFilter, this.companyNameSearchFilter, this.userNameSearchFilter, this.fullNameSearchFilter, this.page, this.size, this.sortBy, this.sortDimension).subscribe(result => {
      this.totalRecords = result.totalCount;
      this.rowData = result.items;
      this.isShowButtonExport = this.rowData.length > 0;
      this.isInitGrid = false;
      this.recordsPerPage = 10;
    });
  }

  getRowIndex(index) {
    return (index + 1) + ((this.page - 1) * this.recordsPerPage);
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

  onRowClickCommon(event) {
    this.selectedItem = event;
    this.selectedItem.oldCompanyCode = event.companyCode;
    this.selectedItem.oldStoreCode = event.storeCode;
    this.selectedItem.oldUserName = event.userName;
    this.selectedStore = this.listStoresAll.find(x => x.storeCode == this.selectedItem.storeCode).storeName;
    this.listStores = this.listStoresAll.filter(x => x.companyId === event.companyId).slice();
    this.selectedCompany = this.listCompanies.find(x => x.companyCode == this.selectedItem.companyCode).companyName;
    this.userPwd = '';
    this.userConfirmPwd = '';
  }
  onPageChangeEvent(event) {
    this.page = event;
    this.load();
  }
  onRowDoubleClickCommon(event) { }
  onExportClickedEvent(event) { }

  handleClearCompany() {
    this.selectedCompany = null;
    this.selectedItem.storeCode = null;
    this.selectedStore = null;
  }
  handleClearStore() {
    this.selectedStore = null;
  }

  filterStores: FlatStoreDto[];

  companiesChanged(event: FlatCompanyDto) {
    this.listStores = [];
    this.selectedItem.storeCode = null;
    this.selectedStore = null;
    if (event) {
      this.selectedCompany = event.companyName;
      this.listStores = this.listStoresAll.filter(x => x.companyId === event.id).slice();
    }
  }

  storesChanged(event: FlatStoreDto) {
    if (!event) {
      return;
    }

    this.selectedStore = event.storeName;
  }

  handleClear() {
    this.selectedItem = {};
    this.userPwd = "";
    this.userConfirmPwd = "";
    this.submitted = false;
    this.selectedCompany = null;
    this.selectedStore = null;
    this.setInputsPristine();
  }

  handleDelete() {
    this.authService.getUser().subscribe(user => {
      if (user.userName == this.selectedItem.userName) {
        this.noticeModalComponent.open(this.translate.instant(`systemManagement.user.cannotSelfDelete`));
      } else {
        this.deleteUser();
      }
    });
  }

  deleteUser() {
    this.confirmModalComponent.open(this.translate.instant('systemManagement.user.deleteConfirm'), this.translate.instant('systemManagement.user.deleteConfirmNotice')).then((isConfirm) => {
      if (isConfirm) {
        this.usersClients.delete(this.selectedItem.id).subscribe(result => {
          if (result.isSuccess) {
            this.noticeModalComponent.open(this.translate.instant(`systemManagement.user.${result.messageCode}`)).then(() => {
              this.handleClear();
              this.load();
            });
          } else {
            this.noticeModalComponent.open(this.translate.instant(`systemManagement.user.${result.messageCode}`));
          }
        }, error => {
          if (error?.response && error?.response !== '') {
            const errorResponseJson = JSON.parse(error.response);
            this.noticeModalComponent.open(this.getErrorMessage(errorResponseJson));
          }
        });
      }
    });
  }

  isSubmit = false;
  inputControl = {};

  handleInputFocus(event, key: string) {
    this.inputControl[key] = event;
  }

  setInputsPristine() {
    for (var property in this.inputControl) {
      if (this.inputControl.hasOwnProperty(property)) {
        this.inputControl[property].control.markAsPristine();
      }
    }
  }

  handleSubmit() {
    if (this.selectedItem.id === null || this.selectedItem.id === undefined) {
      this.selectedItem.password = this.userPwd.trim();
      const bodyValidate = new CreateUserCommand(this.selectedItem);

      if (!this.validateModel(bodyValidate)) {
        this.noticeModalComponent.open(this.translate.instant('systemManagement.common.errorInput'));
        return;
      }

      if (!this.validateConfirmPassword(bodyValidate)) {
        this.noticeModalComponent.open(this.translate.instant('systemManagement.user.passwordNotMatch'));
        return;
      }
    } else {
      this.selectedItem.password = this.userPwd.trim();
      const bodyValidate = new UpdateUserCommand(this.selectedItem);

      if (!this.validateModel(bodyValidate)) {
        this.noticeModalComponent.open(this.translate.instant('systemManagement.common.errorInput'));
        return;
      }

      if (!this.validateConfirmPassword(bodyValidate)) {
        this.noticeModalComponent.open(this.translate.instant('systemManagement.user.passwordNotMatch'));
        return;
      }
    }

    this.isSubmit = true;

    this.confirmCreateUserModal.open(this.selectedItem, this.selectedCompany, this.selectedStore).then((isConfirm) => {
      if (isConfirm) {
        if (this.selectedItem.id === null || this.selectedItem.id === undefined) {
          this.selectedItem.password = this.userPwd.trim();
          this.usersClients.create(this.selectedItem).subscribe(result => {
            if (result.isSuccess) {
              this.noticeModalComponent.open(this.translate.instant(`systemManagement.user.${result.messageCode}`)).then(() => {
                this.handleClear();
                this.load();
              });
            } else {
              this.noticeModalComponent.open(this.translate.instant(`systemManagement.user.${result.messageCode}`));
            }
          }, error => {
            if (error?.response && error?.response !== '') {
              const errorResponseJson = JSON.parse(error.response);
              if (errorResponseJson?.errors?.UserName && errorResponseJson?.errors?.UserName[0] === "createFailLoginID") {
                this.loginid.control.markAsDirty();
                this.loginid.control.setErrors({ 'incorrect': true });
              }
              this.noticeModalComponent.open(this.getErrorMessage(errorResponseJson));
            }
          });
        } else {
          this.selectedItem.password = this.userPwd.trim();
          const body = new UpdateUserCommand(this.selectedItem);
          this.usersClients.update(body).subscribe(result => {
            if (result.isSuccess) {
              this.noticeModalComponent.open(this.translate.instant(`systemManagement.user.${result.messageCode}`)).then(() => {
                this.handleClear();
                this.load();
              });
            } else {
              this.noticeModalComponent.open(this.translate.instant(`systemManagement.user.${result.messageCode}`));
            }
          }, error => {
            if (error?.response && error?.response !== '') {
              const errorResponseJson = JSON.parse(error.response);
              if (errorResponseJson?.errors[''] && errorResponseJson?.errors[''][0] === 'updateFailLoginID') {
                this.loginid.control.markAsDirty();
                this.loginid.control.setErrors({ 'incorrect': true });
              }
              this.noticeModalComponent.open(this.getErrorMessage(errorResponseJson));
            }
          });
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
        errStr = errStr + this.translate.instant(`systemManagement.user.${errorModel.errors[property][0]}`);
      }
    }
    return errStr;
  }

  setInputPristine(element: NgModel) {
    element.control.markAsPristine();
  }

  validateModel(model) {
    let result = true;
    if (!model.companyCode) {
      this.companyCode.control.markAsDirty();
      this.companyCode.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.storeCode) {
      this.storeCode.control.markAsDirty();
      this.storeCode.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.permission) {
      this.permission.control.markAsDirty();
      this.permission.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.fullName.trim()) {
      this.fullName.control.markAsDirty();
      this.fullName.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.userName || !this.userRegex.test(model.userName)) {
      this.loginid.control.markAsDirty();
      this.loginid.control.setErrors({ 'incorrect': true });
      result = false;
    }

    if (model.id === undefined || model.id === null) {
      if (!this.userPwd || !this.pwdRegex.test(this.userPwd)) {
        this.crtUserPwd.control.markAsDirty();
        this.crtUserPwd.control.setErrors({ 'incorrect': true });
        result = false;
      } else {
        result = this.validatePassword(result);
      }
    } else if (this.userPwd) {
      if (!this.pwdRegex.test(this.userPwd)) {
        this.crtUserPwd.control.markAsDirty();
        this.crtUserPwd.control.setErrors({ 'incorrect': true });
        result = false;
      } else {
        result = this.validatePassword(result);
      }
    }

    return result;
  }

  validatePassword(result): boolean {
    if (this.userPwd && this.pwdRegex.test(this.userPwd)) {
      let nonAlphanumeric = this.userPwd.replace(/[0-9a-zA-Z]/gi, '').split('');
      let arrSpecialCharacter = [' ', '!', '”', '#', '$', '%', '&', '’', '(', ')', '*', '+', ',', '-', '.', '/', ':', ';', '<', '>', '=', '?', '@', '[', ']', '\\', '^', '_', '`', '{', '}', '|', '~', '"', '\''];
      if (this.userPwd.length < 8 || this.userPwd.length > 64) {
        this.crtUserPwd.control.markAsDirty();
        this.crtUserPwd.control.setErrors({ 'incorrect': true });
        result = false;
      } else {
        let invalidPassword = false;
        nonAlphanumeric.forEach(char => {
          if (!arrSpecialCharacter.includes(char)) invalidPassword = true;
        });
        if (invalidPassword) {
          this.crtUserPwd.control.markAsDirty();
          this.crtUserPwd.control.setErrors({ 'incorrect': true });
          result = false;
        }
      }
    }
    return result;
  }

  validateConfirmPassword(model) {
    let result = true;
    if (!this.userConfirmPwd || this.userPwd !== this.userConfirmPwd) {
      if (model.id === undefined || model.id === null) {
        this.crtUserConfirmPwd.control.markAsDirty();
        this.crtUserConfirmPwd.control.setErrors({ 'incorrect': true });
        result = false;
      }
    }

    return result;
  }
}
