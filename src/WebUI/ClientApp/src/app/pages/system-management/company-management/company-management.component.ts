import { Component, OnInit, ViewChild } from '@angular/core';
import { NgModel } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { PermissionEnum } from 'src/app/share/constants/enum.constants';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';
import { ConfirmModalComponent } from 'src/app/share/modal/confirm-modal/confirm-modal.component';
import { NoticeModalComponent } from 'src/app/share/modal/notice-modal/notice-modal.component';
import { ApplicationUserDto, CompaniesClient, CompanyDto, CreateCompanyCommand, DeleteCompanyCommand, UpdateCompanyCommand } from 'src/app/web-api-client';
import { ConfirmCreateCompanyModalComponent } from './comfirm-create-company/confirm-create-company.component';

@Component({
  selector: 'app-company-management',
  templateUrl: './company-management.component.html',
  styleUrls: ['./company-management.component.scss'],
})
export class CompanyManagementComponent implements OnInit {
  @ViewChild('closeModal') private closeModalComponent: CloseModalComponent;
  @ViewChild('noticeModal') private noticeModalComponent: NoticeModalComponent;
  @ViewChild('confirmModal') private confirmModalComponent: ConfirmModalComponent;
  @ViewChild('commonGrid') commonGrid;
  @ViewChild('confirmCreateCompanyModal') private confirmCreateCompanyModalComponent: ConfirmCreateCompanyModalComponent;
  @ViewChild('companyCodeValid') companyCodeValid: NgModel;
  @ViewChild('companyNameValid') companyNameValid: NgModel;
  @ViewChild('normalizedCompanyNameValid') normalizedCompanyNameValid: NgModel;
  @ViewChild('companyStatusValid') companyStatusValid: NgModel;
  @ViewChild('orderValid') orderValid: NgModel;

  public permission = PERMISSION;
  public loggedInUser: ApplicationUserDto;
  public permissionEnum = PermissionEnum;
  public companyDto: CompanyDto;
  public info = '';
  public companyCode: any;
  public companyName: any;

  public companyCodeFilter: any;
  public companyNameFilter: any;

  companyIdSelected: any;
  companyCodeSelected: any;
  companyNameSelected: any;
  normalizedCompanyNameSelected: any;
  displayFlagSelected = null;
  orderSelected: any;
  updateAtSelected: any;
  isActiveCreate: boolean;
  inValidCompanyCode: boolean;
  checkValidateInput: boolean;

  companyStatus: any[];
  isInitGrid = true;
  rowData: any = [];
  page = 1;
  totalRecords = 0;


  recordsPerPage = 10;
  orderBy = 'Order';
  orderType = 'asc';
  columnDefs = [
    { field: 'id', hide: true },
    { headerName: 'No.', minWidth: 70, maxWidth: 90, cellRenderer: node => this.getRowIndex(node.rowIndex) },
    { field: 'companyCode', headerName: '会社コード', sortable: true, comparator: () => { } },
    { field: 'companyName', headerName: '会社名称', sortable: true, comparator: () => { } },
    { field: 'normalizedCompanyName', headerName: '会社略称', sortable: true, comparator: () => { } },
    {
      field: 'isActive', headerName: '表示フラグ', sortable: true, comparator: () => { }, valueFormatter: function (params) {
        var dateObj = params.value ? "表示" : "非表示";
        return dateObj;
      }
    },
    { field: 'order', headerName: '表示順', sortable: true, comparator: () => { } }
  ];

  constructor (
    private router: Router,
    private authService: AuthorizeService,
    private companiesClient: CompaniesClient,
    private translate: TranslateService,
  ) { }

  ngOnInit() {
    this.isActiveCreate = true;
    this.inValidCompanyCode = false;
    this.checkValidateInput = false;
    this.companyDto = new CompanyDto();
    this.companyStatus = [
      {
        "status": true,
        "name": "表示"
      },
      {
        "status": false,
        "name": "非表示"
      }];
  }

  getRowIndex(index) {
    return (index + 1) + ((this.page - 1) * this.recordsPerPage);
  }

  onRowDoubleClickCommon(event) { }

  onRowClickCommon(event) {
    this.isActiveCreate = false;
    this.companyIdSelected = null;
    this.companyCodeSelected = null;
    this.companyNameSelected = null;
    this.normalizedCompanyNameSelected = null;
    this.displayFlagSelected = null;
    this.orderSelected = null;
    this.updateAtSelected = null;

    this.companyIdSelected = event.id;
    this.companyCodeSelected = event.companyCode;
    this.companyNameSelected = event.companyName;
    this.normalizedCompanyNameSelected = event.normalizedCompanyName;
    this.displayFlagSelected = event.isActive;
    this.orderSelected = event.order;
    this.updateAtSelected = event?.updatedAt ? new Date(new Date(event?.updatedAt).getTime() - (new Date(event?.updatedAt).getTimezoneOffset() * 60000)) : null;
  }

  searchData() {
    this.mapDataForSearch();
    this.load();
  }

  load() {
    this.companiesClient.getCompaniesWithPagination(this.companyCodeFilter, this.companyNameFilter, this.orderBy, this.orderType, this.page, this.recordsPerPage)
      .subscribe(res => {
        this.rowData = res.items;
        this.totalRecords = res.totalCount;
        this.isInitGrid = false;
      });
  }

  mapDataForSearch(): void {
    this.companyCodeFilter = this.companyCode;
    this.companyNameFilter = this.companyName;
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

    this.load();
  }

  onPageChangeEvent(event) {
    this.page = event;
    this.load();
  }

  validateModel(model) {
    let result = true;
    const codeRegex = /^([0-9]{4})?$/;
    const orderRegex = /^[0-9]*$/;
    if (!codeRegex.test(model.companyCode)) {
      this.companyCodeValid.control.markAsDirty();
      this.companyCodeValid.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.companyName) {
      this.companyNameValid.control.markAsDirty();
      this.companyNameValid.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.normalizedCompanyName) {
      this.normalizedCompanyNameValid.control.markAsDirty();
      this.normalizedCompanyNameValid.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (model.isActive == undefined || model.isActive == null) {
      this.companyStatusValid.control.markAsDirty();
      this.companyStatusValid.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!orderRegex.test('' + model.order)) {
      this.orderValid.control.markAsDirty();
      this.orderValid.control.setErrors({ 'incorrect': true });
      result = false;
    }

    return result;
  }

  checkstatus(event) {
    this.displayFlagSelected = event.status;
  }

  confirmCreateCompany() {

    let createCompanyCommand = new CreateCompanyCommand();
    createCompanyCommand.companyCode = this.companyCodeSelected;
    createCompanyCommand.companyName = this.companyNameSelected;
    createCompanyCommand.normalizedCompanyName = this.normalizedCompanyNameSelected;
    createCompanyCommand.isActive = this.displayFlagSelected;
    createCompanyCommand.order = this.orderSelected;

    if (!this.validateModel(createCompanyCommand)) {
      this.noticeModalComponent.open(this.translate.instant('systemManagement.common.errorInput'));
      return;
    }

    this.confirmCreateCompanyModalComponent.open(createCompanyCommand).then(res => {
      if (res) {
        this.createCompany(createCompanyCommand);
      }
    });
  }

  createCompany(createCompanyCommand: CreateCompanyCommand) {
    this.companiesClient.create(createCompanyCommand).subscribe(res => {
      this.closeModalComponent.open(this.translate.instant('systemManagement.company.createSuccessMessage')).then(() => {
        this.clearData();
        this.load();
      });
    }, error => {
      if (error?.response) {
        const errorResponseJson = JSON.parse(error?.response);
        if (errorResponseJson?.errors['CompanyCode'] && errorResponseJson?.errors['Order']) {
          this.noticeModalComponent.open(this.translate.instant('systemManagement.company.existCompanyCodeMessage') + '<br/>' + this.translate.instant('systemManagement.company.existCompanyOrderMessage'));

          this.companyCodeValid.control.markAsDirty();
          this.companyCodeValid.control.setErrors({ 'incorrect': true });
          this.orderValid.control.markAsDirty();
          this.orderValid.control.setErrors({ 'incorrect': true });

        } else if (errorResponseJson?.errors['CompanyCode'] && !errorResponseJson?.errors['Order']) {
          this.noticeModalComponent.open(this.translate.instant('systemManagement.company.existCompanyCodeMessage'));

          this.companyCodeValid.control.markAsDirty();
          this.companyCodeValid.control.setErrors({ 'incorrect': true });
        } else if (!errorResponseJson?.errors['CompanyCode'] && errorResponseJson?.errors['Order']) {
          this.noticeModalComponent.open(this.translate.instant('systemManagement.company.existCompanyOrderMessage'));

          this.orderValid.control.markAsDirty();
          this.orderValid.control.setErrors({ 'incorrect': true });
        }
      }
    });
  }

  confirmUpdateCompany() {
    this.confirmModalComponent.open(this.translate.instant('systemManagement.company.confirmUpdateMessage')).then(res => {
      if (res) {
        this.updateCompany();
      }
    });
  }

  updateCompany() {
    let updateCompanyCommand = new UpdateCompanyCommand();
    updateCompanyCommand.id = this.companyIdSelected;
    updateCompanyCommand.companyCode = this.companyCodeSelected;
    updateCompanyCommand.companyName = this.companyNameSelected;
    updateCompanyCommand.normalizedCompanyName = this.normalizedCompanyNameSelected;
    updateCompanyCommand.isActive = this.displayFlagSelected;
    updateCompanyCommand.order = this.orderSelected;
    updateCompanyCommand.updatedAt = this.updateAtSelected;

    if (!this.validateModel(updateCompanyCommand)) {
      this.noticeModalComponent.open(this.translate.instant('systemManagement.common.errorInput'));
      return;
    }

    this.companiesClient.update(this.companyIdSelected, updateCompanyCommand).subscribe(res => {
      this.closeModalComponent.open(this.translate.instant('systemManagement.company.updateSuccessMessage')).then(() => {
        this.clearData();
        this.load();
      });
    }, error => {
      if (error?.response) {
        const errorResponseJson = JSON.parse(error?.response);
        if (errorResponseJson?.title === "CompanyCodeExisted") {
          this.noticeModalComponent.open(this.translate.instant('systemManagement.company.existCompanyCodeMessage'));
          this.companyCodeValid.control.markAsDirty();
          this.companyCodeValid.control.setErrors({ 'incorrect': true });
        }
        else if (errorResponseJson?.title === "OrderExisted") {
          this.noticeModalComponent.open(this.translate.instant('systemManagement.company.existCompanyOrderMessage'));
          this.orderValid.control.markAsDirty();
          this.orderValid.control.setErrors({ 'incorrect': true });
        }
        else if (errorResponseJson?.title === "DataChanged") {
          this.noticeModalComponent.open(this.translate.instant('historyInquiry.dataChanged'));
          this.clearData();
        }
        else if (errorResponseJson?.title === "EntityDeleted") {
          this.noticeModalComponent.open(this.translate.instant('historyInquiry.entityDeleted'));
          this.clearData();
        }
      }
    });
  }

  confirmDeleteCompany() {
    this.confirmModalComponent.open(this.translate.instant('systemManagement.company.confirmDeleteMessage'),
      this.translate.instant('systemManagement.company.confirmDeleteNotice')).then(res => {
        if (res) {
          this.deleteCompany();
        }
      });
  }

  deleteCompany() {
    let deleteCompanyCommand = new DeleteCompanyCommand();
    deleteCompanyCommand.id = this.companyIdSelected;
    deleteCompanyCommand.updatedAt = this.updateAtSelected;

    this.companiesClient.delete(this.companyIdSelected, deleteCompanyCommand).subscribe(res => {
      this.closeModalComponent.open(this.translate.instant('systemManagement.company.deleteSuccessMessage')).then(() => {
        this.clearData();
        this.load();
      });
    }, error => {
      if (error?.response) {
        const errorResponseJson = JSON.parse(error?.response);
        if (errorResponseJson?.title === "DataChanged") {
          this.noticeModalComponent.open(this.translate.instant('historyInquiry.dataChanged'));
          this.clearData();
        }
        else if (errorResponseJson?.title === "StoreRegistered") {
          this.noticeModalComponent.open(this.translate.instant('systemManagement.company.existStoredUseCompany'));
          this.clearData();
        } else if (errorResponseJson?.title === "EntityDeleted") {
          this.noticeModalComponent.open(this.translate.instant('systemManagement.company.entityDeleted'));
          this.clearData();
        }
      }
    });
  }

  checkValidCompanyCode() {
    if (this.checkValidateInput) {
      if (this.companyCodeSelected?.length != 4) {
        this.inValidCompanyCode = true;
      }
      else {
        this.inValidCompanyCode = false;
      }
    }
  }

  clearData() {
    this.checkValidateInput = false;
    this.inValidCompanyCode = false;
    this.isActiveCreate = true;
    this.companyIdSelected = null;
    this.companyCodeSelected = null;
    this.companyNameSelected = null;
    this.normalizedCompanyNameSelected = null;
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
  handleInputCompanyNameFocusOut(data) {
    this.companyNameSelected = data.trim();
  }
  handleInputCompanyNameNormalFocusOut(data) {
    this.normalizedCompanyNameSelected = data.trim();
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
