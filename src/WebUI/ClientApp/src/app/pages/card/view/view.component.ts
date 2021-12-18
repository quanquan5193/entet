import { Component, OnInit, ViewChild } from "@angular/core";
import { Location } from "@angular/common";
import { ActivatedRoute } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { AuthorizeService } from "src/app/authorization/authorize.service";
import { DATA } from "src/app/share/constants/data.constants";
import { PermissionEnum } from "src/app/share/constants/enum.constants";
import { PERMISSION } from "src/app/share/constants/permission.constants";
import { NoticeModalComponent } from "src/app/share/modal/notice-modal/notice-modal.component";
import { CardsClient, CardDto, ApplicationUserDto } from "src/app/web-api-client";
@Component({
  selector: "app-view-card-table",
  templateUrl: "./view.component.html",
  styleUrls: ["./view.component.scss"],
})
export class ViewCardComponent implements OnInit {
  @ViewChild("noticeModal") private noticeModalComponent: NoticeModalComponent;
  public permission = PERMISSION;
  public id;
  public isEdit = false;
  cardItem: CardDto;
  canEdit = false;
  constructor (
    private route: ActivatedRoute,
    private cardClient: CardsClient,
    private authService: AuthorizeService,
    private translate: TranslateService,
    private location: Location
  ) { }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get("id");
    this.cardClient.get(this.id).subscribe((data: CardDto) => {
      this.cardItem = data;
      this.canEdit = data && data.company && data.company.isActive && data.store && data.store.isActive;
    }, (error) => {
      const errorResponse = error.response;
      const errorResponseJson = JSON.parse(errorResponse);
      if (errorResponseJson.status === 404) {
        this.noticeModalComponent.open(this.translate.instant('card.entityDeletedEdit')).then(() => {
          this.location.back();
        });
      }
    });
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

  goBack() {
    this.location.back();
  }

  handleClickEdit() {
    if (!this.canEdit) {
      this.noticeModalComponent.open(this.translate.instant('card.cannotEdit'));
      return;
    }
    this.cardClient.checkDeleted(this.id).subscribe((isDeleted) => {
      if (isDeleted) {
        this.noticeModalComponent.open(this.translate.instant('card.entityDeletedEdit'));
        return;
      }
      this.authService.navigateToUrl(`/card/${this.id}/edit`);
    });
  }
}
