import { Component, Injectable, Input, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { NgbModal, NgbModalConfig, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { CreateCompanyCommand } from 'src/app/web-api-client';

@Component({
  selector: 'confirm-create-company',
  templateUrl: './confirm-create-company.component.html',
  styleUrls: ['./confirm-create-company.component.scss']
})
@Injectable()
export class ConfirmCreateCompanyModalComponent implements OnInit {
  public content = '';
  public notice: string;
  public option: any;
  public companyEntity: CreateCompanyCommand;
  public companyActiveStatus: any;
  @ViewChild('modal') private confirmModalContent: TemplateRef<ConfirmCreateCompanyModalComponent>;
  private modalRef: NgbModalRef;
  constructor (
    private modalService: NgbModal,
    config: NgbModalConfig
  ) {
    config.backdrop = 'static';
  }

  ngOnInit(): void { }

  open(createCompanyCommand: any): Promise<boolean> {
    this.companyEntity = createCompanyCommand;
    this.companyActiveStatus = createCompanyCommand.isActive ? "表示" : "非表示";
    let sizeDialog = (this.companyEntity?.companyName?.length > 16 || this.companyEntity?.normalizedCompanyName?.length > 16) ? 'lg' : null;
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
