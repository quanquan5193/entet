import { Component, OnInit, ViewChild } from '@angular/core';
import { NgModel } from '@angular/forms';
import { Location } from "@angular/common";
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';
import { ConfirmModalComponent } from 'src/app/share/modal/confirm-modal/confirm-modal.component';
import { NoticeModalComponent } from 'src/app/share/modal/notice-modal/notice-modal.component';
import { ApplicationUserDto, CardDto, CompaniesClient, CompaniesVm, DevicesClient, FlatCompanyDto, FlatStoreDto, GetDeviceDto, StoresClient, StoresVm } from 'src/app/web-api-client';
import { CloseDeviceModalComponent } from '../closeDeviceModal/close-device-modal.component';
import { PermissionEnum } from 'src/app/share/constants/enum.constants';

@Component({
  selector: 'app-edit-device-management-table',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss'],
})
export class EditDeviceManagementComponent implements OnInit {
  public id;
  public isEdit = false;
  public permission = PERMISSION;
  cardItem: CardDto;
  model: GetDeviceDto;
  deviceStatus: any[];
  listCompanies: FlatCompanyDto[];
  listStores: FlatStoreDto[];
  listStoresAll: FlatStoreDto[];
  notExistMessage = "レコードが存在しません";
  companySelected: any;
  storeSelected: any;
  companyName = '';
  storeName = '';
  primitiveStatus: boolean;
  pattern = /^(\d*\.)?\d+$/;
  isUserRole8 = false;
  private permissionEnum = PermissionEnum;
  private loggedInUser: ApplicationUserDto;


  @ViewChild('confirmModal') private confirmModalComponent: ConfirmModalComponent;
  @ViewChild('noticeModal') private noticeModalComponent: NoticeModalComponent;
  @ViewChild('closeModal') private closeModalComponent: CloseModalComponent;
  @ViewChild('closeDeviceModal') private closeDeviceModalComponent: CloseDeviceModalComponent;
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
    this.model = new GetDeviceDto();
  }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
    this.deviceStatus = [
      {
        "status": true,
        "name": "正常"
      },
      {
        "status": false,
        "name": "無効"
      }];
    this.getDevice();
    this.authService.getUser().subscribe((val) => {
      this.loggedInUser = val;
      if(this.loggedInUser['rolePermission'][this.permission.WF934] == this.permissionEnum.Forbidden){
        this.isUserRole8 = true;
      } else {
        this.isUserRole8 = false;
      }
    });
  }

  getDevice() {
    this.devicesClient.getDevice(this.id).subscribe(res => {
      if (res == null) {
        this.noticeModalComponent.open("レコードが存在しません").then(() => {
          this.goBack();
        });
      }
      else {
        this.model = res;
        this.primitiveStatus = this.model.isActive;
        this.companiesClient.getCompanies().subscribe((data: CompaniesVm) => {
          this.listCompanies = data.lists;
          const lstCompany = this.listCompanies.filter(x => x.id == res.companyId);
          this.companySelected = lstCompany.length != 1 ? null : this.listCompanies.filter(x => x.id == res.companyId)[0]?.companyCode;
          this.companyName = lstCompany.length != 1 ? null : this.listCompanies.filter(x => x.id == res.companyId)[0]?.companyName;
          this.storesClient.getStores().subscribe((data: StoresVm) => {
            this.listStoresAll = data.lists;
            this.listStores = this.filterListStore(res.companyId);
            this.storeSelected = lstCompany.length != 1 ? null : this.listStores.filter(x => x.id == res.storeId)[0]?.storeCode;
            this.storeName = lstCompany.length != 1 ? null : this.listStores.filter(x => x.id == res.storeId)[0]?.storeName;
          });
        });
      }
    }, (error) => {
      this.noticeModalComponent.open("レコードが存在しません").then(() => {
        this.goBack();
      });
    });
  }

  handleCompanySelected(event) {
    this.companySelected = event.companyCode;
    this.companyName = event.companyName;
    this.listStores = this.filterListStore(event.id);
    this.model.storeId = null;
    this.storeName = '';
  }

  handleStoreSelected(event) {
    this.storeSelected = event.storeCode;
    this.storeName = event.storeName;
  }

  filterListStore(id: number) {
    return this.listStoresAll.filter(n => n.companyId == id);
  }

  confirmUpdate() {
    if (this.companySelected == null || this.storeSelected == null) {
      return;
    }

    this.confirmModalComponent.open(this.translate.instant('systemManagement.device.confirmUpdate')).then(res => {
      if (res) {
        this.updateDevice();
      }
    });
  }

  updateDevice() {
    let updatedDate = (this.model.updatedAt == null || this.model.updatedAt == undefined) ? null : new Date(this.model.updatedAt.getTime() - (this.model.updatedAt.getTimezoneOffset() * 60000));
    this.devicesClient.updateDevice(this.model.id, this.companySelected, this.storeSelected, this.model.isActive, this.model.isAutoLock, this.model.lat, this.model.long, updatedDate).subscribe(
      res => {
        this.closeModalComponent.open(this.translate.instant('systemManagement.device.editSuccess')).then(() => {
          this.getDevice();
        });
      }, (error) => {
        const errorResponse = error.response;
        const errorResponseJson = JSON.parse(errorResponse);
        if (errorResponseJson.title === "DataChanged") {
          this.noticeModalComponent.open(this.translate.instant('systemManagement.device.dataChanged'));
        } else if (errorResponseJson.title === "EntityDeleted") {
          this.noticeModalComponent.open(this.translate.instant('systemManagement.device.entityDeleted')).then(() => {
            this.goBack();
          });
        } else if (errorResponseJson.title === "EntityDeleted") {
          this.noticeModalComponent.open(this.translate.instant('systemManagement.device.entityDeleted')).then(() => {
            this.goBack();
          });
        } else if (errorResponseJson.status === 400) {
          this.noticeModalComponent.open(this.getErrorMessage(errorResponseJson));
        }
      });
  }

  goBack() {
    this.location.back();
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

  showConfirmDelete() {
    this.confirmModalComponent.open(this.translate.instant('systemManagement.device.confirmDeleteContent'),
      this.translate.instant('systemManagement.device.confirmDeleteNotice'))
      .then(confirm => {
        if (confirm) {
          this.devicesClient.deleteDevice(this.model.id).subscribe(res => {
            this.noticeModalComponent.open(this.translate.instant('systemManagement.device.deleteSuccess')).then(() => {
              this.goBack();
            });
          }, (error) => {
            const errorResponse = error.response;
            const errorResponseJson = JSON.parse(errorResponse);
            if (errorResponseJson.detail === "DataChanged") {
              this.noticeModalComponent.open(this.translate.instant('systemManagement.device.dataChanged'));
            } else if (errorResponseJson.detail === "ItemNotFounded") {
              this.noticeModalComponent.open(this.translate.instant('systemManagement.device.entityDeleted')).then(() => {
                this.goBack();
              });
            } else if (errorResponseJson.title === "EntityDeleted") {
              this.noticeModalComponent.open(this.translate.instant('systemManagement.device.entityDeleted')).then(() => {
                this.goBack();
              });
            }
          });
        }
      });
  }

  announceMessageWhenChangeStatus() {
    let option = [];
    let isLatLongValid = true;
    if ((this.model.lat?.length > 0 && !this.pattern.test(this.model.lat))) {
      isLatLongValid = false;
      this.lat.control.markAsDirty();
      this.lat.control.setErrors({ 'incorrect': true });
    }

    if ((this.model.long?.length > 0 && !this.pattern.test(this.model.long))) {
      isLatLongValid = false;
      this.long.control.markAsDirty();
      this.long.control.setErrors({ 'incorrect': true });
    }

    if (!isLatLongValid) {
      option.push(this.translate.instant('systemManagement.device.invalidLatLong'));
    }

    if (option.length > 0) {
      if (this.model.isActive != this.primitiveStatus) {
        let noticeOjbect = this.handleStatusNotice();
        this.closeDeviceModalComponent.open(noticeOjbect.firstNotice, noticeOjbect.secondNotice, option);
      }
      else {
        this.closeDeviceModalComponent.open(null, null, option);
      }

    }
    else if (this.model.isActive != this.primitiveStatus) {
      let noticeOjbect = this.handleStatusNotice();
      this.confirmModalComponent.open(noticeOjbect.firstNotice, noticeOjbect.secondNotice)
        .then(res => {
          if (res) {
            this.updateDevice();
          }
        });
    }
    else {
      this.confirmUpdate();
    }
  }

  handleStatusNotice() {
    let firstNotice = null;
    let secondNotice = null;
    if (!this.model.isActive) {
      firstNotice = this.translate.instant('systemManagement.device.firstDisableDeviceWarning');
      secondNotice = this.translate.instant('systemManagement.device.secondDisableDeviceWarning');
    }
    else {
      firstNotice = this.translate.instant('systemManagement.device.firstEnableDeviceWarning');
      secondNotice = this.translate.instant('systemManagement.device.secondEnableDeviceWarning');
    }
    return { "firstNotice": firstNotice, "secondNotice": secondNotice };
  }

  handleClickEdit() {
    this.isEdit = !this.isEdit;
  }

  handleStoreChange(event) {
    this.storeName = event.storeName;
    this.storeSelected = event.storeCode;
  }

  handleCompanyChange(event) {
    this.companyName = event.companyName;
    this.listStores = this.filterListStore(event.id);
    this.storeName = '';
  }

  setInputPristine(element: NgModel) {
    element.control.markAsPristine();
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
