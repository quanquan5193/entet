import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({ selector: '[japaneseLayoutDatepickerDirective]' })
export class JapaneseLayoutDatepickerDirective {
  calendarObject: any;
  constructor(private el: ElementRef) { }
  detectIE() {
    const ua = window.navigator.userAgent;
    const msie = ua.indexOf('MSIE ');
    if (msie > 0) {
      return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
    }

    const trident = ua.indexOf('Trident/');
    if (trident > 0) {
      const rv = ua.indexOf('rv:');
      return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
    }

    const edge = ua.indexOf('Edge/');
    if (edge > 0) {
      return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
    }
    return false;
  }
  @HostListener('click')
  onClick() {
    const version = this.detectIE();

    if (version === false) {
      setTimeout(() => {
        this.calendarObject = document.querySelector('ngb-datepicker-navigation-select');
        if (this.calendarObject !== null) {
          const monthObject = this.calendarObject.children[0];
          const yearObject = this.calendarObject.children[1];
          (<HTMLElement>monthObject).style.marginLeft = '78px';
          (<HTMLElement>monthObject).style.marginRight = '135px';
          (<HTMLElement>yearObject).style.marginLeft = '-304px';
          (<HTMLElement>yearObject).style.marginRight = '78px';
        }
      }, 0);
    } else if (version >= 12) {
      setTimeout(() => {
        this.calendarObject = document.querySelector('ngb-datepicker-navigation-select');
        if (this.calendarObject !== null) {
          const monthObject = this.calendarObject.children[0];
          const yearObject = this.calendarObject.children[1];
          (<HTMLElement>monthObject).style.width = '70px';
          (<HTMLElement>monthObject).style.marginLeft = '92px';
          (<HTMLElement>monthObject).style.marginRight = '100px';
          (<HTMLElement>monthObject).style.position = 'absolute';
          (<HTMLElement>yearObject).style.marginLeft = '-5px';
          (<HTMLElement>yearObject).style.marginRight = '65px';
          (<HTMLElement>yearObject).style.position = 'relative';
        }
      }, 0);
    } else {
      setTimeout(() => {
        this.calendarObject = document.querySelector('ngb-datepicker-navigation-select');
        if (this.calendarObject !== null) {
          const monthObject = this.calendarObject.children[0];
          const yearObject = this.calendarObject.children[1];
          (<HTMLElement>monthObject).style.width = '70px';
          (<HTMLElement>monthObject).style.marginLeft = '92px';
          (<HTMLElement>monthObject).style.marginRight = '100px';
          (<HTMLElement>monthObject).style.position = 'absolute';
          (<HTMLElement>yearObject).style.marginLeft = '-5px';
          (<HTMLElement>yearObject).style.marginRight = '65px';
          (<HTMLElement>yearObject).style.position = 'relative';
        }
      }, 0);
    }
  }
}
