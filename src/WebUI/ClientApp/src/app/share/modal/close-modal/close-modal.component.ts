import { Component, Injectable, Input, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { NgbModal, NgbModalRef, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'close-modal',
  templateUrl: './close-modal.component.html',
  styleUrls: ['./close-modal.component.scss']
})
@Injectable()
export class CloseModalComponent implements OnInit {
  public content = '';
  @ViewChild('modal') private closeModalContent: TemplateRef<CloseModalComponent>;
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
