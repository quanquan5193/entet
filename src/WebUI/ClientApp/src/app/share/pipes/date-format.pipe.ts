import { Pipe, PipeTransform } from '@angular/core';
import * as dayjs from 'dayjs';
import { CONFIG } from '../constants/config.constants';

@Pipe({
  name: 'dateFormatPipe'
})
export class DateFormatPipe implements PipeTransform {
  transform(value: any): any {
    if (value) {
      return dayjs(value).format(CONFIG.DateFormat);
    } else {
      return '';
    }
  }
}
