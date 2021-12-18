import { Component, Injectable, Input, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { NgbModal, NgbModalConfig, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { CreateStoreCommand } from 'src/app/web-api-client';

@Component({
  selector: 'confirm-create-store',
  templateUrl: './confirm-create-store.component.html',
  styleUrls: ['./confirm-create-store.component.scss']
})
@Injectable()
export class ConfirmCreateStoreModalComponent implements OnInit {
  public content = '';
  public notice: string;
  public option: any;
  public storeEntity: CreateStoreCommand;
  public storeActiveStatus: any;
  @ViewChild('modal') private confirmModalContent: TemplateRef<ConfirmCreateStoreModalComponent>;
  private modalRef: NgbModalRef;
  constructor (
    private modalService: NgbModal,
    config: NgbModalConfig
  ) {
    config.backdrop = 'static';
  }

  ngOnInit(): void { }

  open(createStoreCommand: any): Promise<boolean> {
    this.storeEntity = createStoreCommand;
    this.storeActiveStatus = createStoreCommand.isActive ? "表示" : "非表示";
    let sizeDialog = (this.storeEntity?.storeName?.length > 16 || this.storeEntity?.normalizedStoreName?.length > 16) ? 'lg' : null;
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
