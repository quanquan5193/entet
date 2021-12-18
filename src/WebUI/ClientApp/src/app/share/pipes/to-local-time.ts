import { Pipe, PipeTransform } from '@angular/core';
import * as dayjs from 'dayjs';
import { CONFIG } from '../constants/config.constants';

@Pipe({
  name: 'toLocalTime'
})
export class ToLocalDateFormatPipe implements PipeTransform {
  transform(value: any): any {
    if (value) {
      let theDate = this.adjustForTimezone(new Date(Date.parse(value)));
      let options = {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
      } as const;

      return theDate.toLocaleString('ja-JP', options);
    } else {
      return '';
    }
  }

  adjustForTimezone(date: Date): Date {
    // date is UTC date
    // get client Timezone
    var timeOffsetInMS: number = date.getTimezoneOffset() * 60000;
    // add Timezone to UTC
    date.setTime(date.getTime() - timeOffsetInMS);
    return date;
  }
}
