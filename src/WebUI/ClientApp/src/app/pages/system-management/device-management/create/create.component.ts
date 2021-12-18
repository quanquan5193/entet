import { Component, OnInit, ViewChild } from '@angular/core';
import { NgModel } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Location } from "@angular/common";
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';
import { ConfirmModalComponent } from 'src/app/share/modal/confirm-modal/confirm-modal.component';
import { NoticeModalComponent } from 'src/app/share/modal/notice-modal/notice-modal.component';
import { CardDto, CompaniesClient, CompaniesVm, CreateDeviceCommand, DevicesClient, FlatCompanyDto, FlatStoreDto, StoresClient, StoresVm } from 'src/app/web-api-client';
import { ConfirmCreateModalComponent } from './confirm-modal/confirm-create-modal.component';

@Component({
  selector: 'app-create-device-management-table',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.scss'],
})
export class CreateDeviceManagementComponent implements OnInit {
  public isEdit = false;
  public permission = PERMISSION;
  cardItem: CardDto;
  deviceStatus: any[];
  listCompanies: FlatCompanyDto[];
  listStores: FlatStoreDto[];
  isSearchStore = false;
  notExistMessage = "レコードが存在しません";
  companySelected: any;
  storeSelected: any;
  deviceEntity: CreateDeviceCommand;
  isCreate = false;
  companyName = '';
  storeName = '';
  listStoresAll: FlatStoreDto[];

  @ViewChild('confirmModal') private confirmModalComponent: ConfirmModalComponent;
  @ViewChild('confirmCreateModal') private confirmCreateModalComponent: ConfirmCreateModalComponent;
  @ViewChild('noticeModal') private noticeModalComponent: NoticeModalComponent;
  @ViewChild('closeModal') private closeModalComponent: CloseModalComponent;
  @ViewChild('deviceCodeInput') deviceCodeInput: NgModel;
  @ViewChild('companySelect') companySelect: NgModel;
  @ViewChild('storeSelect') storeSelect: NgModel;
  @ViewChild('deviceSelect') deviceSelect: NgModel;
  @ViewChild('lat') lat: NgModel;
  @ViewChild('long') long: NgModel;

  constructor (
    private route: ActivatedRoute,
    private devicesClient: DevicesClient,
    private storesClient: StoresClient,
    private companiesClient: CompaniesClient,
    private authService: AuthorizeService,
    private translate: TranslateService,
    private location: Location
  ) {
    this.deviceEntity = new CreateDeviceCommand();
    this.deviceEntity.isAutoLock = true;
  }

  ngOnInit() {
    this.deviceStatus = [
      {
        "status": true,
        "name": "正常"
      },
      {
        "status": false,
        "name": "無効"
      }];

    this.companiesClient.getCompanies().subscribe((data: CompaniesVm) => {
      this.listCompanies = data.lists;
    });
    this.storesClient.getStores().subscribe((data: StoresVm) => {
      this.listStoresAll = data.lists;
      this.listStores = data.lists;
    });
  }

  filterListStore(id: number) {
    return this.listStoresAll.filter(n => n.companyId == id);
  }

  handleCompanyChange(event) {
    this.companyName = event.companyName;
    this.listStores = this.filterListStore(event.id);
    this.storeName = '';
    this.deviceEntity.storeCode = null;
  }

  handleStoreChange(event, isLatValid, isLongValid) {
    this.storeName = event.storeName;
  }

  confirmUpdate() {
    this.confirmCreateModalComponent.open(this.deviceEntity, this.companyName, this.storeName).then(res => {
      if (res) {
        this.updateDevice();
      }
    });
  }

  goBack() {
    this.location.back();
  }

  updateDevice() {

    if (!this.validateModel(this.deviceEntity)) {
      this.noticeModalComponent.open(this.translate.instant('systemManagement.common.errorInput'));
      return;
    }

    this.devicesClient.createDevice(this.deviceEntity).subscribe(res => {
      this.closeModalComponent.open(this.translate.instant('systemManagement.device.createSuccess')).then(() => {
        this.authService.navigateToUrl(`/system-management/device-management/${res}/edit`);
      });
    }, error => {
      const errorResponse = error.response;
      const errorResponseJson = JSON.parse(errorResponse);
      if (errorResponseJson.title === "DataChanged") {
        this.noticeModalComponent.open("errorResponseJson.title");
      } else if (errorResponseJson.title === "EntityDeleted") {
        this.noticeModalComponent.open(this.translate.instant('systemManagement.device.entityDeleted')).then(() => {
          this.authService.navigateToUrl(`/system-management/device-management`);
        });
      } else if (errorResponseJson.status == 400 && errorResponseJson.errors['DeviceCode']) {
        this.noticeModalComponent.open(this.translate.instant('systemManagement.device.deviceCodeExist'));
        this.deviceCodeInput.control.markAsDirty();
        this.deviceCodeInput.control.setErrors({ 'incorrect': true });
      } else if (errorResponseJson.status == 400) {
        this.noticeModalComponent.open(this.getErrorMessage(errorResponseJson));
      }
    });
  }

  handleClickEdit() {
    this.isEdit = !this.isEdit;
  }

  getErrorMessage(errorModel): string {
    let errStr = '';
    for (var property in errorModel.errors) {
      if (errorModel.errors.hasOwnProperty(property)) {
        if (errStr !== '') {
          errStr = errStr + '<br/>';
        }
        errStr = errStr + this.translate.instant(`systemManagement.device.${errorModel.errors[property][0]}`);
      }
    }
    return errStr;
  }

  setInputPristine(element: NgModel) {
    element.control.markAsPristine();
  }

  validateModel(model) {
    let result = true;
    const codeRegex = /^[a-zA-Z0-9]{1,20}$/;
    const latRegex = /^(\d*\.)?\d+$/;
    const longRegex = /^(\d*\.)?\d+$/;
    if (!codeRegex.test(model.deviceCode)) {
      this.deviceCodeInput.control.markAsDirty();
      this.deviceCodeInput.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.companyCode) {
      this.companySelect.control.markAsDirty();
      this.companySelect.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.storeCode) {
      this.storeSelect.control.markAsDirty();
      this.storeSelect.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.deviceStatus) {
      this.deviceSelect.control.markAsDirty();
      this.deviceSelect.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (!model.deviceStatus) {
      this.deviceSelect.control.markAsDirty();
      this.deviceSelect.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (model.lat?.length > 0 && !latRegex.test(model.lat)) {
      this.lat.control.markAsDirty();
      this.lat.control.setErrors({ 'incorrect': true });
      result = false;
    }
    if (model.long?.length > 0 && !longRegex.test(model.long)) {
      this.long.control.markAsDirty();
      this.long.control.setErrors({ 'incorrect': true });
      result = false;
    }
    return result;
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
