import { OnInit, ElementRef, Input, Directive, Renderer2 } from '@angular/core';
import { AuthorizeService } from 'src/app/authorization/authorize.service';
import { ApplicationUserDto } from 'src/app/web-api-client';
import { PermissionEnum } from '../constants/enum.constants';
@Directive({
  selector: '[checkPermission]'
})
export class PermissionCheckDirective implements OnInit {
  permissions: any;
  _valueCheck: any;
  private loggedInUser: ApplicationUserDto;
  private permissionEnum = PermissionEnum;

  @Input() checkPermission: any;

  get valueCheck(): any {
    return this._valueCheck;
  }

  @Input()
  set valueCheck(val: any) {
    this._valueCheck = val;
    if (this._valueCheck !== undefined) {
      this.processCheckPermission();
    }
  }

  constructor (
    private elementRef: ElementRef,
    private renderer2: Renderer2,
    private authService: AuthorizeService,
  ) { }

  ngOnInit() {
    this.authService.getUser().subscribe((val) => {
      this.loggedInUser = val;
    });
    this.processCheckPermission();
  }

  processCheckPermission() {
    if (this.checkPermission) {
      let fields = this.checkPermission.split('|');
      if (this.loggedInUser['rolePermission'][fields[0]] == this.permissionEnum.Forbidden) {
        if (fields[1]) {
          this.disableElement();
        } else {
          this.removeElement();
        }
        return;
      }
    } else {
      return;
    }
  }

  removeElement() {
    // this.renderer2.removeChild(this.elementRef.nativeElement.parentNode, this.elementRef.nativeElement);
    this.renderer2.addClass(this.elementRef.nativeElement, 'd-none');
  }

  disableElement() {
    setTimeout(() => {
      // add them
      if (this.elementRef.nativeElement.localName === 'span') {
        return;
      }
      this.renderer2.addClass(this.elementRef.nativeElement, 'readonly');
      if (this.elementRef.nativeElement.localName === 'a') {
        this.renderer2.addClass(this.elementRef.nativeElement, 'btn-disable');
      }
    });
  }
}
