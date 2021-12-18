import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { PermissionEnum } from 'src/app/share/constants/enum.constants';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { NoticeModalComponent } from 'src/app/share/modal/notice-modal/notice-modal.component';
import { ApplicationUserDto, CardDto, KidDetailDto, KidsClubClient } from 'src/app/web-api-client';
import { Location } from "@angular/common";

@Component({
  selector: 'app-view-kid-club-table',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss'],
})
export class ViewKidClubComponent implements OnInit {
  public id;
  public canEdit = false;
  public permission = PERMISSION;
  kid: KidDetailDto;
  @ViewChild("noticeModal") private noticeModalComponent: NoticeModalComponent;


  constructor (
    private route: ActivatedRoute,
    private kidsClubClient: KidsClubClient,
    private authService: AuthorizeService,
    private translate: TranslateService,
    private location: Location
  ) { }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
    this.kidsClubClient.getKid(this.id).subscribe(data => {
      this.canEdit = data.isEnableEdit;
      this.kid = data;
    },
      error => {
        if (error?.response && error?.response !== '') {
          this.noticeModalComponent.open(this.translate.instant('kidClub.entityDeleted')).then(() => {
            this.authService.navigateToUrl('/kid-club');
          });
        }
      });
  }

  goBack() {
    this.location.back();
  }

  handleClickEdit() {
    if (!this.canEdit) {
      this.noticeModalComponent.open(this.translate.instant('kidClub.errorMessage.editFailIsNotActive'));
      return;
    }
    this.authService.navigateToUrl(`/kid-club/${this.id}/edit`);
  }
}
