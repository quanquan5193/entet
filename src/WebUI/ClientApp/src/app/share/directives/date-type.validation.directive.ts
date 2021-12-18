import { Component, OnDestroy, OnInit, ElementRef, Input, Output, EventEmitter, Directive, OnChanges } from '@angular/core';
import { NG_VALIDATORS, Validator, FormControl, ValidatorFn, AbstractControl } from '@angular/forms';
import { CONFIG } from '../constants/config.constants';
import * as dayjs from 'dayjs';

function validateDateTypeFactory(): ValidatorFn {
  return (c: AbstractControl) => {
    if (
      c.value === null ||
      c.value === '' ||
      c.value === undefined ||
      (c.value && c.value.trim && c.value.trim() === '') ||
      dayjs(c.value, CONFIG.DateFormat, true).isValid()
    ) {
      return null;
    } else {
      const dateRegex = new RegExp(CONFIG.DateFormatRegex);
      if (!dateRegex.test(c.value)) {
        return { dateFormat: true };
      }

      return {
        date: true
      };
    }
  };
}

@Directive({
  selector: '[validationDateType][ngModel]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: DateTypeValidatorDirective,
      multi: true
    }
  ]
})
export class DateTypeValidatorDirective implements Validator {
  validator: ValidatorFn;

  constructor() {
    this.validator = validateDateTypeFactory();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }
}
