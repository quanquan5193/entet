import { Component, Injectable, Input, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { NgbModal, NgbModalConfig, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'confirm-create-user',
  templateUrl: './confirm-create-user.component.html',
  styleUrls: ['./confirm-create-user.component.scss']
})
@Injectable()
export class ConfirmCreateUserModalComponent implements OnInit {
  public content = '';
  public notice: string;
  public option: any;
  public userEntity: any;
  public userActiveStatus: any;
  public createdDate = new Date(Date.now());
  public companyName = "";
  public storeName = "";
  @ViewChild('modal') private confirmModalContent: TemplateRef<ConfirmCreateUserModalComponent>;
  private modalRef: NgbModalRef;
  constructor(
    private modalService: NgbModal,
    config: NgbModalConfig
  ) {
    config.backdrop = 'static';
  }

  ngOnInit(): void { }

  open(createUserCommand: any,company: string,store: string): Promise<boolean> {
    this.userEntity = createUserCommand;
    this.companyName = company;
    this.storeName = store;
    this.userActiveStatus = createUserCommand.isActive ? "表示" : "非表示";
    return new Promise<boolean>(resolve => {
      this.modalRef = this.modalService.open(this.confirmModalContent, {
        centered: true
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
