import { RequestsReceiptedDetailsDto, RequestsReceiptedsClient, RequestType, Member, Device, Store, Company, PICStore } from './../../../web-api-client';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NoticeModalComponent } from 'src/app/share/modal/notice-modal/notice-modal.component';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { serializeNodes } from '@angular/compiler/src/i18n/digest';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-view-history-inquiry-table',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss'],
})
export class ViewHistoryInquiryComponent implements OnInit {
  model: RequestsReceiptedDetailsDto;
  public id;
  public canEdit = false;
  public permission = PERMISSION;
  zipCodeFirtPart = null;
  zipCodeSecondPart = null;
  //TODO: message hasn't defined yet
  noticeMessage = "レコードが存在しません";
  @ViewChild('noticeModal') private noticeModalComponent: NoticeModalComponent;
  constructor (
    private route: ActivatedRoute,
    private client: RequestsReceiptedsClient,
    private authService: AuthorizeService,
    private translate: TranslateService
  ) {
    this.model = new RequestsReceiptedDetailsDto();
    this.model.requestType = new RequestType();
    this.model.member = new Member();
    this.model.member.picStore = new PICStore();
    this.model.device = new Device();
    this.model.device.store = new Store();
    this.model.device.store.company = new Company();
  }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
    this.client.getRequestsReceipted(this.id).subscribe((res) => {
      if (res == null) {
        this.noticeModalComponent.open(this.noticeMessage).then(() => {
          this.authService.navigateToUrl(`/history-inquiry`);
        });
      } else {
        this.canEdit = res && res.device && res.device.store && res.device.store.isActive && res.device.store.company && res.device.store.company.isActive;
        this.model = res;
        if (this.model?.member?.zipcodeId) {
          this.zipCodeFirtPart = this.model.member.zipcodeId.substring(0, 3).toString();
          this.zipCodeSecondPart = this.model.member.zipcodeId.substring(3, 7).toString();
        }
      }
    }, (error) => {
    });
  }

  handleClickEdit() {
    if (!this.canEdit) {
      this.noticeModalComponent.open(this.translate.instant('historyInquiry.error.editFailIsNotActive'));
      return;
    }
    this.authService.navigateToUrl(`/history-inquiry/${this.id}/edit`);
  }
}
