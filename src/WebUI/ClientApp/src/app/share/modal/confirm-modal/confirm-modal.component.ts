import { Component, Injectable, Input, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { NgbModal, NgbModalConfig, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';

@Component({
  selector: 'confirm-modal',
  templateUrl: './confirm-modal.component.html',
  styleUrls: ['./confirm-modal.component.scss']
})
@Injectable()
export class ConfirmModalComponent implements OnInit {
  public content = '';
  public notice: string;
  public option: any;
  @ViewChild('modal') private confirmModalContent: TemplateRef<ConfirmModalComponent>;
  private modalRef: NgbModalRef;
  constructor(
    private modalService: NgbModal,
    config: NgbModalConfig
  ) {
    config.backdrop = 'static';
  }

  ngOnInit(): void { }

  open(content: string, notice?: string, option?: any): Promise<boolean> {
    this.content = content;
    this.notice = notice;
    this.option = option;
    return new Promise<boolean>(resolve => {
      this.modalRef = this.modalService.open(this.confirmModalContent, {
        centered: true,
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
