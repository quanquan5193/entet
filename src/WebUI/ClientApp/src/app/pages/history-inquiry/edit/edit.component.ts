import { GetListPersonInchargeQuery, MemberInfoDto, MembersClient, PersonInchargeDto, SexType, UpdateHistoryCommand } from './../../../web-api-client';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { ConfirmModalComponent } from 'src/app/share/modal/confirm-modal/confirm-modal.component';
import { NoticeModalComponent } from 'src/app/share/modal/notice-modal/notice-modal.component';
import { Company, Device, Member, RequestsReceiptedDetailsDto, RequestsReceiptedsClient, RequestType, Store, CardsClient } from 'src/app/web-api-client';
import { TranslateService } from '@ngx-translate/core';
import { NgModel } from '@angular/forms';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';

@Component({
  selector: 'app-edit-history-inquiry-table',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss'],
})
export class EditHistoryInquiryComponent implements OnInit, AfterViewInit {
  public id;
  public isEdit = false;
  public permission = PERMISSION;
  // TODO: message hasn't defined yet
  notExistMessage = "レコードが存在しません";
  confirmMessage = "申請内容を削除しますか?";
  subConfirmMessage = "※削除した場合、復元できません。";
  subConfirmMessageColor = "yellow";
  noticeMessage = "お客様情報を削除しました。";
  @ViewChild('confirmModal') private confirmModalComponent: ConfirmModalComponent;
  @ViewChild('noticeModal') private noticeModalComponent: NoticeModalComponent;
  @ViewChild('closeModal') private closeModalComponent: CloseModalComponent;
  @ViewChild("memberNo") private memberNoControl: NgModel;
  @ViewChild("oldMemberNo") private oldMemberNoControl: NgModel;
  @ViewChild("furiganaLastName") private furiganaLastNameControl: NgModel;
  @ViewChild("furiganaFirstName") private furiganaFirstNameControl: NgModel;
  @ViewChild("email") private emailControl: NgModel;
  @ViewChild("fixedPhone") private fixedPhoneControl: NgModel;
  @ViewChild("mobilePhone") private mobilePhoneControl: NgModel;
  @ViewChild("zipcodeFirstPart") private zipcodeFirstPartControl: NgModel;
  @ViewChild("zipcodeSecondPart") private zipcodeSecondPartControl: NgModel;
  @ViewChild("dateOfBirth") private dateOfBirthControl: NgModel;
  dateOfBirth: NgbDateStruct;
  model: RequestsReceiptedDetailsDto;
  doesReceiveAds: boolean;
  sex: number;
  zipCodeLeft: string = null;
  zipCodeRight: string = null;
  listPIC: PersonInchargeDto[];
  listSex = [SexType.Male, SexType.Female, SexType.Other];

  constructor (
    private route: ActivatedRoute,
    private client: RequestsReceiptedsClient,
    private authService: AuthorizeService,
    private membersClient: MembersClient,
    private translate: TranslateService,
    public formatter: NgbDateParserFormatter,
    private cardsClient: CardsClient
  ) {
    this.model = new RequestsReceiptedDetailsDto();
    this.model.requestType = new RequestType();
    this.model.member = new Member();
    this.model.device = new Device();
    this.model.device.store = new Store();
    this.model.device.store.company = new Company();
  }
  ngAfterViewInit(): void {
    this.markAsDirtyControl();
  }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
    this.client.getRequestsReceipted(this.id).subscribe((res) => {
      if (res == null) {
        this.noticeMessage = this.notExistMessage;
        this.noticeModalComponent.open(this.noticeMessage).then(() => {
          this.authService.navigateToUrl(`/history-inquiry`);
        });
      } else {
        if (!(res && res.device && res.device.store && res.device.store.isActive && res.device.store.company && res.device.store.company.isActive)) this.authService.navigateToUrl('/access-denied');
        this.model = res;
        if (this.model != null && this.model.member != null && this.model.member.zipcodeId != null && this.model.member.zipcodeId.length == 7) {
          this.zipCodeLeft = this.model.member.zipcodeId.slice(0, 3);
          this.zipCodeRight = this.model.member.zipcodeId.slice(3);
        }

        if (this.model != null && this.model.member != null) {
          this.membersClient.getListPersonInchargeCMS(this.model.member.createdBy).subscribe(res => {
            this.listPIC = res;
          });
        }
      }
    }, (error) => {
    });
  }

  showConfirmDelete() {
    this.confirmModalComponent.open(this.confirmMessage, this.subConfirmMessage).then(res => {
      if (res) {
        this.client.delete(this.id).subscribe(res => {
          this.noticeModalComponent.open(this.noticeMessage).then(() => {
            this.authService.navigateToUrl(`/history-inquiry`);
          });
        }, (error) => {
          this.noticeModalComponent.open(this.notExistMessage);
        });
      }
    });
  }

  getZipCodeInfo() {
    let isValid = true;
    const leftZipCodeRegex = /^[0-9]{3}$/;
    if (!leftZipCodeRegex.test(this.zipCodeLeft)) {
      this.zipcodeFirstPartControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.zipcodeFirstPartControl.control.markAsPristine();
    }

    const rightZipCodeRegex = /^[0-9]{4}$/;
    if (!rightZipCodeRegex.test(this.zipCodeRight)) {
      this.zipcodeSecondPartControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.zipcodeSecondPartControl.control.markAsPristine();
    }

    if (!isValid) {
      this.noticeModalComponent.open(this.translate.instant('historyInquiry.error.inValidInput'));
      return;
    }

    this.membersClient.getInfoZipcode(this.zipCodeLeft + this.zipCodeRight).subscribe(res => {
      if (res != null) {
        this.model.member.province = res.province;
        this.model.member.district = res.district;
        this.model.member.street = res.street;
      } else {
        this.model.member.province = '';
        this.model.member.district = '';
        this.model.member.street = '';
      }
    }, err => {
      this.closeModalComponent.open(this.translate.instant('system.error.fetchDataFail'));
    });
  }

  async confirmUpdate() {
    const isValidInput = await this.validateInputValue();
    if (!isValidInput) {
      this.noticeModalComponent.open(this.translate.instant('historyInquiry.error.inValidInput'));
      return;
    }

    this.confirmModalComponent.open(this.translate.instant('historyInquiry.confirmUpdate')).then(res => {
      if (res) {
        this.saveData();
      }
    });
  }

  async validateInputValue(): Promise<boolean> {
    let isValid = true;
    this.markAsDirtyControl();
    const memberNoRegex = /^[0,1,2,3,6,8]{1}[0-9]{9}$/;
    if (!memberNoRegex.test(this.model.member.memberNo)) {
      this.memberNoControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      if (this.model.member.memberNo[0] == '0' && this.model.member.memberNo[1] == '9') {
        this.memberNoControl.control.setErrors({ 'incorrect': true });
        isValid = false;
      } else {
        this.memberNoControl.control.markAsPristine();
      }
    }

    //if is point migration
    if (this.model.requestType.id === 6) {
      await this.cardsClient.checkCardPointMigrationGivePoint(this.model.member.oldMemberNo, this.model.member.memberNo).toPromise().then((res) => {
        if (res !== "OK") {
          this.oldMemberNoControl.control.setErrors({ 'incorrect': true });
          isValid = false;
        }
      }, (error) => { });
    }
    else if (this.model.requestType.id === 3) {
      await this.cardsClient.checkCardForReissueLostCard(this.model.member.oldMemberNo).toPromise().then((res) => {
        if (res !== "OK") {
          this.oldMemberNoControl.control.setErrors({ 'incorrect': true });
          isValid = false;
        }
      }, (error) => {

      });
    }
    else {
      if (this.model.member.oldMemberNo != null && this.model.member.oldMemberNo != undefined && !memberNoRegex.test(this.model.member.oldMemberNo)) {
        this.oldMemberNoControl.control.setErrors({ 'incorrect': true });
        isValid = false;
      } else {
        if (this.model.member.oldMemberNo != null && this.model.member.oldMemberNo != undefined) {
          if (this.model.member.oldMemberNo[0] == '0' && this.model.member.oldMemberNo[1] == '9') {
            this.oldMemberNoControl.control.setErrors({ 'incorrect': true });
            isValid = false;
          } else {
            this.oldMemberNoControl.control.markAsPristine();
          }
        }
        else {
          this.oldMemberNoControl.control.markAsPristine();
        }
      }
    }


    const katakanaRegex = /^([ァ-ン]|ー)+$/;
    if (this.model.member.furiganaLastName && !katakanaRegex.test(this.model.member.furiganaLastName)) {
      this.furiganaLastNameControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.furiganaLastNameControl.control.markAsPristine();
    }

    if (this.model.member.furiganaFirstName && !katakanaRegex.test(this.model.member.furiganaFirstName)) {
      this.furiganaFirstNameControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.furiganaFirstNameControl.control.markAsPristine();
    }

    const fixedPhoneRegex = /^[0-9]{10,11}$/;
    const mobilePhoneRegex = /^[0-9]{11}$/;

    if (this.model.member.fixedPhone?.length > 0) {
      if (!fixedPhoneRegex.test(this.model.member.fixedPhone) || this.model.member.fixedPhone[0] != '0') {
        this.fixedPhoneControl.control.setErrors({ 'incorrect': true });
        isValid = false;
      } else {
        this.fixedPhoneControl.control.markAsPristine();
      }
    }
    if (this.model.member.mobilePhone?.length > 0) {
      if (!mobilePhoneRegex.test(this.model.member.mobilePhone) || this.model.member.mobilePhone[0] != '0') {
        this.mobilePhoneControl.control.setErrors({ 'incorrect': true });
        isValid = false;
      } else {
        this.mobilePhoneControl.control.markAsPristine();
      }
    }

    const emailRegex = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (this.model.member.email && !emailRegex.test(this.model.member.email)) {
      this.emailControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.emailControl.control.markAsPristine();
    }

    if (this.zipCodeLeft && !this.zipCodeRight) {
      this.zipcodeSecondPartControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else if (!this.zipCodeLeft && this.zipCodeRight) {
      this.zipcodeFirstPartControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      const leftZipCodeRegex = /^[0-9]{3}$/;
      if (this.zipCodeLeft && !leftZipCodeRegex.test(this.zipCodeLeft)) {
        this.zipcodeFirstPartControl.control.setErrors({ 'incorrect': true });
        isValid = false;
      } else {
        this.zipcodeFirstPartControl.control.markAsPristine();
      }

      const rightZipCodeRegex = /^[0-9]{4}$/;
      if (this.zipCodeRight && !rightZipCodeRegex.test(this.zipCodeRight)) {
        this.zipcodeSecondPartControl.control.setErrors({ 'incorrect': true });
        isValid = false;
      } else {
        this.zipcodeSecondPartControl.control.markAsPristine();
      }
    }

    return isValid;
  }

  markAsDirtyControl() {
    this.memberNoControl.control.markAsDirty();
    this.oldMemberNoControl.control.markAsDirty();
    this.furiganaLastNameControl.control.markAsDirty();
    this.furiganaFirstNameControl.control.markAsDirty();
    this.emailControl.control.markAsDirty();
    this.fixedPhoneControl.control.markAsDirty();
    this.mobilePhoneControl.control.markAsDirty();
    this.zipcodeFirstPartControl.control.markAsDirty();
    this.zipcodeSecondPartControl.control.markAsDirty();
    this.dateOfBirthControl.control.markAsDirty();
  }

  setSexValue(sex) {
    this.model.member.sex = sex;
  }

  setElementDirty(element: NgModel) {
    element.control.markAsPristine();
  }

  validateDate(date) {
    const parsed = this.formatter.parse(date.target.value);
    if (parsed == null) {
      this.model.member.dateOfBirth = null;
      return;
    }
    if (parsed.year < 1900 || parsed.year > 2100) {
      this.model.member.dateOfBirth = null;
      return;
    };
    if (parsed.day == null) {
      this.model.member.dateOfBirth = null;
      return;
    }
    const numbers = date.target.value.trim().split('/');
    if (!(+numbers[2])) {
      this.model.member.dateOfBirth = null;
      return;
    }
  }

  saveData() {
    var savedData = new UpdateHistoryCommand();
    savedData.id = Number.parseInt(this.id);
    savedData.picStoreId = this.model.member.picStoreId;
    savedData.remark = this.model.member.remark;
    savedData.updatedAt = this.model.updatedAt == null ? null : new Date(this.model.updatedAt.getTime() - (this.model.updatedAt.getTimezoneOffset() * 60000));
    savedData.memberInfo = new MemberInfoDto();
    savedData.memberInfo.furiganaFirstName = this.model.member?.furiganaFirstName;
    savedData.memberInfo.furiganaLastName = this.model.member?.furiganaLastName;
    savedData.memberInfo.firstName = this.model.member?.firstName;
    savedData.memberInfo.lastName = this.model.member?.lastName;
    savedData.memberInfo.fixedPhone = this.model.member?.fixedPhone;
    savedData.memberInfo.mobilePhone = this.model.member?.mobilePhone;
    if (this.model.member.dateOfBirth !== undefined && this.model.member.dateOfBirth !== null && this.model.member.dateOfBirth.getFullYear() > 1900) {
      savedData.memberInfo.dateOfBirth = this.model.member?.dateOfBirth == null ? null : new Date(this.model.member?.dateOfBirth.getTime() - (this.model.member?.dateOfBirth.getTimezoneOffset() * 60000));
      savedData.memberInfo.dateOfBirth = savedData.memberInfo.dateOfBirth != null && savedData.memberInfo.dateOfBirth.getFullYear() == 0 ? null : savedData.memberInfo.dateOfBirth;
    }
    savedData.memberInfo.sex = this.model.member?.sex;
    savedData.memberInfo.isNetMember = this.model.member.isNetMember;
    savedData.memberInfo.isRegisterAdvertisement = this.model.member.isRegisterAdvertisement;
    savedData.memberInfo.province = this.model.member?.province;
    savedData.memberInfo.city = this.model.member?.district;
    savedData.memberInfo.address = this.model.member?.street;
    savedData.memberInfo.buildingName = this.model.member?.buildingName;
    savedData.memberInfo.email = this.model.member?.email;
    savedData.memberInfo.memberNo = this.model.member?.memberNo;
    savedData.memberInfo.oldMemberNo = this.model.member?.oldMemberNo;
    savedData.memberInfo.isUpdateInformation = this.model.member?.isUpdateInformation;
    
    if(this.zipCodeLeft && this.zipCodeRight){
      savedData.memberInfo.zipcode = this.zipCodeLeft + this.zipCodeRight;
    }

    this.client.update(this.id, savedData).subscribe((res) => {
      this.noticeModalComponent.open(this.translate.instant('historyInquiry.editSuccess')).then(() => {
        this.authService.navigateToUrl(`/history-inquiry/${this.id}`);
      });
    }, (error) => {
      const errorResponse = error.response;
      const errorResponseJson = JSON.parse(errorResponse);


      if (errorResponseJson.detail === "DataChanged") {
        this.noticeModalComponent.open(this.translate.instant('historyInquiry.dataChanged'));
      } else if (errorResponseJson.detail === "EntityDeleted") {
        this.noticeModalComponent.open(this.translate.instant('historyInquiry.entityDeleted')).then(() => {
          this.authService.navigateToUrl(`/history-inquiry`);
        });
      } else if (errorResponseJson.status == 400) {
        for (var property in errorResponseJson.errors) {
          if (errorResponseJson.errors.hasOwnProperty(property)) {
            if (errorResponseJson.errors[property][0] === 'memberNo') {
              if (this.memberNoControl) {
                this.memberNoControl.control.markAsDirty();
                this.memberNoControl.control.setErrors({ 'incorrect': true });
                this.noticeModalComponent.open(this.translate.instant('historyInquiry.error.inValidInput'));
              }
            }
            if (errorResponseJson.errors[property][0] === 'oldMemberNo') {
              if (this.oldMemberNoControl) {
                this.oldMemberNoControl.control.markAsDirty();
                this.oldMemberNoControl.control.setErrors({ 'incorrect': true });
                this.noticeModalComponent.open(this.translate.instant('historyInquiry.error.inValidInput'));
              }
            }
          }
        }
      }
    });
  }
}
