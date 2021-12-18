import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import * as $ from 'jquery';
import { CloseModalComponent } from 'src/app/share/modal/close-modal/close-modal.component';
import { ConfirmModalComponent } from 'src/app/share/modal/confirm-modal/confirm-modal.component';
import { CardsClient } from 'src/app/web-api-client';

@Component({
  selector: 'app-register-cards',
  templateUrl: './register-cards.component.html',
  styleUrls: ['./register-cards.component.scss'],
})
export class RegisterCardsComponent implements OnInit {
  @ViewChild('confirmModal') private confirmModalComponent: ConfirmModalComponent;
  @ViewChild('closeModal') private closeModalComponent: CloseModalComponent;
  public fileUploadName: string;
  public selectedFile: any;
  constructor (
    private listCardsClient: CardsClient,
    private translate: TranslateService
  ) { }

  ngOnInit() {

  }

  getFileDetail(event) {
    if (!this.validateFileExtension(event.target.files[0].name)) {
      this.closeModalComponent.open(this.translate.instant('batchProcess.uploadError'));
      return;
    }
    const inputFile = $('#fileUploadBtn');
    this.selectedFile = event.target.files[0];
    this.fileUploadName = event.target.files[0].name;
    inputFile.val('');
  }

  validateFileExtension(fileName: string) {
    const extension = fileName.substring(fileName.lastIndexOf('.') + 1);
    return extension.toLowerCase() === 'csv';
  }

  handleOpenUploadPopup() {
    const confirmText = this.translate.instant('batchProcess.confirmUpload');
    const noticeText = this.translate.instant('batchProcess.noticeUpload');
    this.confirmModalComponent.open(confirmText, noticeText).then(res => {
      if (res) this.uploadFile();
    });
  }

  uploadFile() {
    const fileParameter = {
      data: this.selectedFile,
      fileName: this.selectedFile.name
    };
    this.listCardsClient.importCards(fileParameter).subscribe(result => {
      if (result > 0) {
        this.closeModalComponent.open(result + this.translate.instant('batchProcess.uploadSuccess'));
        this.deleteFile();
      } else {
        this.onUploadError();
      }
    }, error => {
      this.onUploadError();
    });
  }

  deleteFile() {
    this.selectedFile = undefined;
    this.fileUploadName = '';
  }

  private async onUploadError() {
    await this.closeModalComponent.open(this.translate.instant('batchProcess.uploadError'));
  }
}
