import { Injectable } from '@angular/core';
import { NgbDateParserFormatter, NgbDateStruct, NgbDatepickerI18n } from '@ng-bootstrap/ng-bootstrap';

const I18N_VALUES = {
  ja: {
    weekdays: ['月', '火', '水', '木', '金', '土', '日'],
    months: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
    year: '年'
  }
};

@Injectable()
export class NgBootstrapDateFormatterFactory extends NgbDateParserFormatter {
  parse(value: string): NgbDateStruct {
    if (value) {
      const dateParts = value.trim().split("/");
      if (dateParts.length > 3) return null;
      if (dateParts.length === 1 && isNumber(dateParts[0])) {
        return { year: toInteger(dateParts[0]), month: null, day: null };
      } else if (dateParts.length === 2 && isNumber(dateParts[0]) && isNumber(dateParts[1])) {
        return {
          year: toInteger(dateParts[0]),
          month: toInteger(dateParts[1]),
          day: null
        };
      } else if (dateParts.length === 3 && isNumber(dateParts[0]) && isNumber(dateParts[1]) && isNumber(dateParts[2])) {
        if (!isInputValidDate(dateParts[0], dateParts[1], dateParts[2])) return null;
        return {
          year: toInteger(dateParts[0]),
          month: toInteger(dateParts[1]),
          day: toInteger(dateParts[2])
        };
      }
      return null;
    }
    return null;
  }

  format(date: NgbDateStruct): string {
    return date ? `${date.year}/${isNumber(date.month) ? padNumber(date.month) : ""}/${isNumber(date.day) ? padNumber(date.day) : ""}` : "";
  }
}

export function toInteger(value: any): number {
  return parseInt(`${value}`, 10);
}

export function isNumber(value: any): value is number {
  return !(isNaN(toInteger(value)));
}

export function padNumber(value: number) {
  if (isNumber(value)) {
    return `0${value}`.slice(-2);
  } else {
    return "";
  }
}

export function isInputValidDate(year: string, month: string, day: string): boolean {
  if (!Number.isInteger(+year) || !Number.isInteger(+month) || !Number.isInteger(+day)) return false;
  const yearNumber = parseInt(year);
  const monthNumber = parseInt(month);
  const dayNumber = parseInt(day);
  if (monthNumber < 1 || monthNumber > 12 || dayNumber < 1) return false;
  const isLeapYear = ((yearNumber % 4 == 0) && (yearNumber % 100 != 0)) || (yearNumber % 400 == 0);
  const month31Days = [1, 3, 5, 7, 8, 10, 12];
  const month30Days = [4, 6, 9, 11];
  const month28or29Days = [2];
  if (month31Days.includes(monthNumber) && dayNumber <= 31) return true;
  if (month30Days.includes(monthNumber) && dayNumber <= 30) return true;
  if (month28or29Days.includes(monthNumber) && dayNumber <= (isLeapYear ? 29 : 28)) return true;
  return false;
}

@Injectable()
export class I18n {
  language = 'ja';
}

@Injectable()
export class CustomDatepickerI18n extends NgbDatepickerI18n {
  constructor (private _i18n: I18n) {
    super();
  }

  getWeekdayShortName(weekday: number): string {
    return I18N_VALUES[this._i18n.language].weekdays[weekday - 1];
  }
  getMonthShortName(month: number): string {
    return I18N_VALUES[this._i18n.language].months[month - 1];
  }
  getMonthFullName(month: number): string {
    return this.getMonthShortName(month);
  }

  getDayAriaLabel(date: NgbDateStruct): string {
    return `${date.day}-${date.month}-${date.year}`;
  }

  getYearNumerals(year: number): string {
    return `${year} ${I18N_VALUES[this._i18n.language].year}`;
  }
}
