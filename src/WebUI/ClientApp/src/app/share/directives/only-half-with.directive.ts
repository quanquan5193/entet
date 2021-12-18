import { ElementRef, Input, Directive, HostListener } from '@angular/core';
import { isBuffer } from 'util';

@Directive({
  selector: '[onlyHalfWidth]'
})
export class OnlyHalfWidthDirective {

  regexStr = '^[0-9]*$';
  regexStrDot = '^[0-9.]*$';
  valueOld = "";
  constructor(private el: ElementRef) { }

  @Input() onlyHalfWidth: boolean;
  @Input() allowDot: boolean = false;

  @HostListener('input', ['$event']) onKeyDown(event) {
    if (this.onlyHalfWidth) {
      let regEx = this.allowDot ? new RegExp(this.regexStrDot) : new RegExp(this.regexStr);    
      if(event.inputType === 'deleteContentBackward' || event.inputType === 'deleteByCut') {
        this.valueOld = this.el.nativeElement.value;
        return;
      };

      if(event.inputType === 'insertFromPaste') {
        let temp = this.el.nativeElement.value;
        if(regEx.test(temp))
        {
          this.valueOld = this.el.nativeElement.value;
          return;
        } else
        { 
          this.el.nativeElement.value = this.valueOld;
          event.preventDefault();
          return false;
        }
      };

      if(regEx.test(event.data))
      {
        this.valueOld = this.el.nativeElement.value;
        return;
      }
      else
      { 
        this.el.nativeElement.value = this.valueOld;
        event.preventDefault();
        return false;
      }
    }
  }
}