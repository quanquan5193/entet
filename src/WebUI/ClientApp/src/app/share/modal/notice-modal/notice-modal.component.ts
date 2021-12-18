import { Component, Injectable, Input, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { NgbModal, NgbModalConfig, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';

@Component({
  selector: 'notice-modal',
  templateUrl: './notice-modal.component.html',
  styleUrls: ['./notice-modal.component.scss']
})
@Injectable()
export class NoticeModalComponent implements OnInit {
  public content = '';
  @ViewChild('modal') private noticeModalContent: TemplateRef<NoticeModalComponent>;
  private modalRef: NgbModalRef;
  constructor(
    private modalService: NgbModal,
    config: NgbModalConfig
  ) {
    config.backdrop = 'static';
  }

  ngOnInit(): void { }

  open(content: string): Promise<boolean> {
    this.content = content;
    return new Promise<boolean>(resolve => {
      this.modalRef = this.modalService.open(this.noticeModalContent, {
        centered: true,
      });
      this.modalRef.result.then(resolve, resolve);
    });
  }

  async close(): Promise<void> {
    this.modalRef.close();
  }

  async dismiss(): Promise<void> {
    this.modalRef.dismiss();
  }
}
