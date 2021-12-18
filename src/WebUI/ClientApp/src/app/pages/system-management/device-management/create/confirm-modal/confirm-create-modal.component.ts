import { Component, Injectable, Input, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { NgbModal, NgbModalConfig, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { CreateDeviceCommand } from 'src/app/web-api-client';

@Component({
  selector: 'confirm-create-modal',
  templateUrl: './confirm-create-modal.component.html',
  styleUrls: ['./confirm-create-modal.component.scss']
})
@Injectable()
export class ConfirmCreateModalComponent implements OnInit {
  public content = '';
  public companyName = '';
  public storeName = '';
  public notice: string;
  public option: any;
  public deviceEntity: CreateDeviceCommand;
  public deviceStatus: any;
  public deviceAutoLockStatus: any;
  @ViewChild('modal') private confirmModalContent: TemplateRef<ConfirmCreateModalComponent>;
  private modalRef: NgbModalRef;
  constructor (
    private modalService: NgbModal,
    config: NgbModalConfig
  ) {
    config.backdrop = 'static';
  }

  ngOnInit(): void { }

  open(deviceEntity: any, companyName, storeName): Promise<boolean> {
    this.deviceEntity = deviceEntity;
    this.companyName = companyName;
    this.storeName = storeName;
    this.deviceStatus = deviceEntity.deviceStatus ? "正常" : "無効";
    this.deviceAutoLockStatus = deviceEntity.isAutoLock ? "正常" : "無効";
    let sizeDialog = (this.deviceEntity?.deviceCode?.length > 16 && this.deviceEntity?.lat?.toString().length > 16 || this.deviceEntity?.long?.toString().length > 16) ? 'lg' : null;
    return new Promise<boolean>(resolve => {
      this.modalRef = this.modalService.open(this.confirmModalContent, {
        centered: true,
        size: sizeDialog
      });
      this.modalRef.result.then(resolve, resolve);
    });
  }

  async close(): Promise<void> {
    this.modalRef.close(false);
  }

  async confirm(): Promise<void> {
    this.modalRef.close(true);
  }

}
