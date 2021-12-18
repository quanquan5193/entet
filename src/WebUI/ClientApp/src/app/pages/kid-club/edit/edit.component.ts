import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { NgModel } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { DATA } from 'src/app/share/constants/data.constants';
import { PermissionEnum } from 'src/app/share/constants/enum.constants';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { ConfirmModalComponent } from 'src/app/share/modal/confirm-modal/confirm-modal.component';
import { NoticeModalComponent } from 'src/app/share/modal/notice-modal/notice-modal.component';
import { ApplicationUserDto, GetListPersonInchargeQuery, KidDetailDto, KidRelationshipEnum, KidsClubClient, MembersClient, PersonInchargeDto, SexType, UpdateKidCommand } from 'src/app/web-api-client';

@Component({
  selector: 'app-edit-kid-club-table',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss'],
})
export class EditKidClubComponent implements OnInit, AfterViewInit {
  @ViewChild("confirmModal") private confirmModalComponent: ConfirmModalComponent;
  @ViewChild("noticeModal") private noticeModalComponent: NoticeModalComponent;
  @ViewChild("memberNo") private memberNoControl: NgModel;
  @ViewChild("parentLastName") private parentLastNameControl: NgModel;
  @ViewChild("parentFirstName") private parentFirstNameControl: NgModel;
  @ViewChild("email") private emailControl: NgModel;
  @ViewChild("lastName") private lastNameControl: NgModel;
  @ViewChild("firstName") private firstNameControl: NgModel;
  @ViewChild("furiganaLastName") private furiganaLastNameControl: NgModel;
  @ViewChild("furiganaFirstName") private furiganaFirstNameControl: NgModel;
  @ViewChild("dateOfBirth") private dateOfBirthControl: NgModel;
  public id;
  public canEdit = false;
  public permission = PERMISSION;
  dateModel: any;
  kid: KidDetailDto = new KidDetailDto();
  updateModel: UpdateKidCommand = new UpdateKidCommand();
  model: NgbDateStruct;
  listSex: any[];
  listRelation: any[];
  listPIC: PersonInchargeDto[];

  getListSexForEdit(status: SexType) {
    return Object.values(SexType)
      .filter((k) => typeof k === "number")
      .map((a) => a);
  }

  getListRelationForEdit(status: KidRelationshipEnum) {
    return Object.values(KidRelationshipEnum)
      .filter((k) => typeof k === "number")
      .map((a) => a);
  }

  handleFilterSex(type) {
    switch (type) {
      case DATA.SexType.Male:
        return '男';
      case DATA.SexType.Female:
        return '女';
      case DATA.SexType.Other:
        return 'その他';
    }
  }

  handleFilterRelation(type) {
    switch (type) {
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
  constructor (
    private route: ActivatedRoute,
    private kidsClubClient: KidsClubClient,
    private membersClient: MembersClient,
    private authService: AuthorizeService,
    private translate: TranslateService,
    public formatter: NgbDateParserFormatter) {
    const data = [];
  }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
    this.kidsClubClient.getKid(this.id).subscribe(data => {
      if (!data.isEnableEdit) this.authService.navigateToUrl('/access-denied');
      this.kid = data;
      this.listSex = this.getListSexForEdit(this.kid.sex);
      this.listRelation = this.getListRelationForEdit(this.kid.relationshipMember);

      if (this.kid != null && this.kid.deviceCode != null) {
        this.membersClient.getListPersonInchargeCMS(this.kid.createdBy).subscribe(res => {
          this.listPIC = res;
        });
      }
    },
      error => {
        if (error?.response && error?.response !== '') {
          this.noticeModalComponent.open(this.translate.instant('kidClub.editFail')).then(() => {
            this.authService.navigateToUrl('/kid-club');
          });
        }
      });
  }

  ngAfterViewInit(): void {
    this.markAsDirtyControl();
  }

  handleQueryDate(date: Date) {
    return new Date(date.getTime() - (date.getTimezoneOffset() * 60000));
  }

  onClickConfirm() {
    const isValidInput = this.validateInputValue();
    if (!isValidInput) {
      this.noticeModalComponent.open(this.translate.instant('kidClub.errorMessage.inputInvalidPopup'));
      return;
    }

    this.confirmModalComponent.open(this.translate.instant('kidClub.confirmEdit')).then((isConfirm) => {
      if (isConfirm) {
        this.updateModel.id = this.kid.id;
        this.updateModel.memberNo = this.kid.memberNo;
        this.updateModel.parentFirstName = this.kid.parentFirstName;
        this.updateModel.parentLastName = this.kid.parentLastName;
        this.updateModel.relationshipMember = this.kid.relationshipMember;
        this.updateModel.email = this.kid.email;
        this.updateModel.picStoreId = this.kid.picStoreId;
        this.updateModel.firstName = this.kid.firstName;
        this.updateModel.lastName = this.kid.lastName;
        this.updateModel.furiganaFirstName = this.kid.furiganaFirstName;
        this.updateModel.furiganaLastName = this.kid.furiganaLastName;
        this.updateModel.dateOfBirth = this.handleQueryDate(this.kid.dateOfBirth);
        this.updateModel.sex = this.kid.sex;
        this.updateModel.isLivingWithParent = this.kid.isLivingWithParent;
        this.updateModel.remark = this.kid.remark;
        this.updateModel.updatedAt = this.kid.updatedAt != undefined ? this.handleQueryDate(this.kid.updatedAt) : this.kid.updatedAt;

        this.kidsClubClient.updateKid(this.updateModel).subscribe(
          (result) => {
            if (result.isSuccess) {
              this.noticeModalComponent.open(this.translate.instant('kidClub.editSuccess')).then(() => {
                this.authService.navigateToUrl(`/kid-club/${this.kid.id}`);
              });
            } else {
              this.noticeModalComponent.open(this.translate.instant(`kidClub.${result.messageCode}`));
              if (result.messageCode === 'cardInvalid') {
                this.memberNoControl.control.setErrors({ 'incorrect': true });
                this.memberNoControl.control.markAsDirty();
              }
            }
          },
          error => {
            if (error?.response && error?.response !== '') {
              const errorResponseJson = JSON.parse(error.response);
              if (errorResponseJson.status === 400) {
                this.memberNoControl.control.setErrors({ 'incorrect': true });
                this.memberNoControl.control.markAsDirty();
                this.noticeModalComponent.open(this.getErrorMessage(errorResponseJson));
              } else {
                this.noticeModalComponent.open(this.translate.instant('kidClub.editFail'));
              }
            }
          }
        );
      }
    });
  }

  validateInputValue(): boolean {
    let isValid = true;
    const memberNoRegex = /^[ｦ-ﾟ,0,1,2,3,6,8]{1}[0-9]{9}$/;
    this.markAsDirtyControl();
    if (!memberNoRegex.test(this.kid.memberNo)) {
      this.memberNoControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      if (this.kid.memberNo[0] == '0' && this.kid.memberNo[1] == '9') {
        this.memberNoControl.control.setErrors({ 'incorrect': true });
        isValid = false;
      }
      else {
        this.memberNoControl.control.markAsPristine();
      }
    }

    if (!this.kid.parentLastName) {
      this.parentLastNameControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.parentLastNameControl.control.markAsPristine();
    }

    if (!this.kid.parentFirstName) {
      this.parentFirstNameControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.parentFirstNameControl.control.markAsPristine();
    }

    const emailRegex = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (this.kid.email?.length > 0 && !emailRegex.test(this.kid.email)) {
      this.emailControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.emailControl.control.markAsPristine();
    }

    if (!this.kid.lastName) {
      this.lastNameControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.lastNameControl.control.markAsPristine();
    }

    if (!this.kid.firstName) {
      this.firstNameControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.firstNameControl.control.markAsPristine();
    }

    const katakanaRegex = /^([ァ-ン]|ー)+$/;
    if (!katakanaRegex.test(this.kid.furiganaLastName)) {
      this.furiganaLastNameControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.furiganaLastNameControl.control.markAsPristine();
    }

    if (!katakanaRegex.test(this.kid.furiganaFirstName)) {
      this.furiganaFirstNameControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.furiganaFirstNameControl.control.markAsPristine();
    }

    if (!this.kid.dateOfBirth) {
      this.dateOfBirthControl.control.setErrors({ 'incorrect': true });
      isValid = false;
    } else {
      this.dateOfBirthControl.control.markAsPristine();
    }

    return isValid;
  }

  markAsDirtyControl() {
    this.memberNoControl.control.markAsDirty();
    this.parentLastNameControl.control.markAsDirty();
    this.parentFirstNameControl.control.markAsDirty();
    this.emailControl.control.markAsDirty();
    this.lastNameControl.control.markAsDirty();
    this.firstNameControl.control.markAsDirty();
    this.furiganaLastNameControl.control.markAsDirty();
    this.furiganaFirstNameControl.control.markAsDirty();
    this.dateOfBirthControl.control.markAsDirty();
  }

  validateDate(date) {
    const parsed = this.formatter.parse(date.target.value);
    if (parsed == null) {
      this.kid.dateOfBirth = null;
      return;
    }
    if (parsed.year < 1900 || parsed.year > 2100) {
      this.kid.dateOfBirth = null;
      return;
    };
    if (parsed.day == null) {
      this.kid.dateOfBirth = null;
      return;
    }
    const numbers = date.target.value.trim().split('/');
    if (!(+numbers[2])) {
      this.kid.dateOfBirth = null;
      return;
    }
  }

  setElementDirty(element: NgModel) {
    element.control.markAsPristine();
  }

  onClickDelete() {
    this.confirmModalComponent.open(this.translate.instant('kidClub.confirmDelete'), this.translate.instant('kidClub.noticeDelete')).then(res => {
      if (res) this.deleteKid();
    });

  }
  deleteKid() {
    this.kidsClubClient.deleteKid(this.id).subscribe((res) => {
      if (res) {
        this.noticeModalComponent.open(this.translate.instant('kidClub.deleteSuccess')).then(() => {
          this.authService.navigateToUrl('/kid-club');
        });
      } else {
        this.noticeModalComponent.open(this.translate.instant('kidClub.deleteFail'));
      }
    },
      error => {
        this.noticeModalComponent.open(this.translate.instant('kidClub.deleteFail'));
      }
    );
  }

  getErrorMessage(errorModel): string {
    let errStr = '';
    for (var property in errorModel.errors) {
      if (errorModel.errors.hasOwnProperty(property)) {
        if (errStr !== '') {
          errStr = errStr + '<br/>';
        }
        errStr = errStr + this.translate.instant(`kidClub.errorMessage.${errorModel.errors[property][0]}`);
      }
    }
    return errStr;
  }

  inputControl = {};

  handleInputFocus(event, key: string) {
    this.inputControl[key] = event;
  }
}
