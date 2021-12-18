import { Component, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Location } from "@angular/common";
import { AuthorizeService } from "src/app/authorization/authorize.service";
import { TranslateService } from '@ngx-translate/core';
import { ConfirmModalComponent } from "src/app/share/modal/confirm-modal/confirm-modal.component";
import { NoticeModalComponent } from "src/app/share/modal/notice-modal/notice-modal.component";
import {
  ApplicationUserDto,
  CardDto,
  CardsClient,
  CardStatus,
  CompaniesClient,
  CompaniesVm,
  FlatCompanyDto,
  FlatStoreDto,
  StoresClient,
  StoresVm,
  UpdateCardCommand,
} from "src/app/web-api-client";
import { DATA } from "src/app/share/constants/data.constants";
import { PermissionEnum } from "src/app/share/constants/enum.constants";
import { PERMISSION } from "src/app/share/constants/permission.constants";

@Component({
  selector: "app-edit-card-table",
  templateUrl: "./edit.component.html",
  styleUrls: ["./edit.component.scss"],
})
export class EditCardComponent implements OnInit {
  @ViewChild("confirmModal") private confirmModalComponent: ConfirmModalComponent;
  @ViewChild("noticeModal") private noticeModalComponent: NoticeModalComponent;
  public permission = PERMISSION;
  public id;
  public isEdit = false;
  cardItem: CardDto = new CardDto();
  listStatus: any[];
  listCompanies: FlatCompanyDto[];
  listStores: FlatStoreDto[];
  listStoresAll: FlatStoreDto[];
  companyName = "";
  companyCode = "";
  companyId: number;
  storeName = "";
  storeCode = "";
  canSearch = false;
  constructor (
    private route: ActivatedRoute,
    private cardClient: CardsClient,
    private companiesClient: CompaniesClient,
    private storesClient: StoresClient,
    private authService: AuthorizeService,
    private translate: TranslateService,
    private location: Location
  ) { }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get("id");
    this.cardClient.get(this.id).subscribe((card: CardDto) => {
      if (!(card && card.company && card.company.isActive && card.store && card.store.isActive)) this.authService.navigateToUrl('/access-denied');
      this.cardItem = card;
      this.companyId = card.companyId;
      this.companyName = card.company?.companyName;
      this.companyCode = card.company?.companyCode;
      this.storeName = card.store?.storeName;
      this.storeCode = card.store?.storeCode;
      this.listStatus = this.getListStatusForEdit(card.status);
      this.companiesClient.getCompanies().subscribe((data: CompaniesVm) => {
        this.listCompanies = data.lists;
        this.storesClient.getStores().subscribe((data: StoresVm) => {
          this.listStoresAll = data.lists;
          this.listStores = this.filterListStore(this.companyId);
        });
      });
    }, (error) => {
      const errorResponse = error.response;
      const errorResponseJson = JSON.parse(errorResponse);
      if (errorResponseJson.status === 404) {
        this.noticeModalComponent.open(this.translate.instant('card.entityDeletedEdit')).then(() => {
          this.goBack();
        });
      }
    });
  }

  goBack() {
    this.location.back();
  }

  handleFilterStatus(status) {
    switch (status) {
      case DATA.CardStatus.Unissued:
        return '未発行';
      case DATA.CardStatus.Issued:
        return '発行済';
      case DATA.CardStatus.Withdrawal:
        return '退会';
      case DATA.CardStatus.Missing:
        return '紛失';
      case DATA.CardStatus.Disposal:
        return '廃棄';
    }
  }

  getListStatusForEdit(status: CardStatus) {
    return Object.values(CardStatus)
      .filter((k) => typeof k === "number" && k >= status)
      .map((a) => a);
  }

  companiesChanged(event: FlatCompanyDto) {
    this.companyName = event.companyName;
    this.companyCode = event.companyCode;
    this.listStores = this.filterListStore(event.id);
    this.storeName = '';
    this.storeCode = '';
    this.cardItem.storeId = null;
  }

  storesChanged(event: FlatStoreDto) {
    this.storeName = event.storeName;
    this.storeCode = event.storeCode;
  }

  filterListStore(id: number) {
    return this.listStoresAll.filter(n => n.companyId == id);
  }

  handleClickEdit() {
    try {
      this.confirmModalComponent.open(this.translate.instant('card.confirmEdit')).then((isConfirm) => {
        if (isConfirm) {
          const command = new UpdateCardCommand();
          command.id = this.cardItem.id;
          command.status = this.cardItem.status;
          const companyCommand = new FlatCompanyDto();
          companyCommand.id = this.cardItem.companyId;
          companyCommand.companyCode = this.companyCode;
          companyCommand.companyName = this.companyName;
          companyCommand.isDeleted = false;
          companyCommand.normalizedCompanyName = "";
          command.company = companyCommand;

          const storeCommand = new FlatStoreDto();
          storeCommand.id = this.cardItem.storeId;
          storeCommand.storeCode = this.storeCode;
          storeCommand.storeName = this.storeName;
          storeCommand.isDeleted = false;
          storeCommand.normalizedStoreName = "";
          command.store = storeCommand;
          this.cardClient.update(this.cardItem.id, command).subscribe(
            (res) => {
              this.noticeModalComponent.open(this.translate.instant('card.editSuccess')).then(() => {
                this.authService.navigateToUrl(`/card/${this.cardItem.id}`);
              });
            },
            (error) => {
              const errorResponse = error.response;
              const errorResponseJson = JSON.parse(errorResponse);
              if (errorResponseJson.title === "DataChanged") {
                this.noticeModalComponent.open(this.translate.instant('card.dataChanged'));
              } else if (errorResponseJson.title === "IsNotActive") {
                this.noticeModalComponent.open(this.translate.instant('card.cannotEdit')).then(() => {
                  this.authService.navigateToUrl(`/card`);
                });
              } else if (errorResponseJson.title === "EntityDeleted") {
                this.noticeModalComponent.open(this.translate.instant('card.entityDeletedEdit')).then(() => {
                  this.authService.navigateToUrl(`/card`);
                });
              }
            }
          );
        }
      });
    } catch (error) {
    }
  }

  handleClickDelete() {
    this.confirmModalComponent.open(this.translate.instant('card.confirmDelete'), this.translate.instant('card.noticeDelete')).then(res => {
      if (res) this.deleteCard();
    });
  }

  deleteCard() {
    this.cardClient.delete(this.id).subscribe(() => {
      this.noticeModalComponent.open(this.translate.instant('card.deleteSuccess')).then(() => {
        this.authService.navigateToUrl('/card');
      });
    }, (error) => {
      const errorResponse = error.response;
      const errorResponseJson = JSON.parse(errorResponse);
      if (errorResponseJson.title === "DataChanged") {
        this.noticeModalComponent.open(this.translate.instant('card.dataChanged'));
      } else if (errorResponseJson.title === "EntityDeleted") {
        this.noticeModalComponent.open(this.translate.instant('card.entityDeletedDelete')).then(() => {
          this.authService.navigateToUrl(`/card`);
        });
      }
    });
  }
}
