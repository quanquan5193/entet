import { Component, Injectable, Input, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { NgbModal, NgbModalRef, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'close-device-modal',
  templateUrl: './close-device-modal.component.html',
  styleUrls: ['./close-device-modal.component.scss']
})
@Injectable()
export class CloseDeviceModalComponent implements OnInit {
  public option: any;
  public firstNotice: string;
  public secondNotice: string;
  @ViewChild('modal') private closeModalContent: TemplateRef<CloseDeviceModalComponent>;
  private modalRef: NgbModalRef;
  constructor(
    private modalService: NgbModal,
    config: NgbModalConfig
  ) {
    config.backdrop = 'static';
  }

  ngOnInit(): void { }

  open(firstNotice?: string, secondNotice?: string, option?: any,): Promise<boolean> {
    this.option = option;
    this.firstNotice = firstNotice;
    this.secondNotice = secondNotice;
    return new Promise<boolean>(resolve => {
      this.modalRef = this.modalService.open(this.closeModalContent, {
        centered: true,
      });
      this.modalRef.result.then(resolve, resolve);
    });
  }

  async close(): Promise<void> {
    this.modalRef.close();
  }
}
