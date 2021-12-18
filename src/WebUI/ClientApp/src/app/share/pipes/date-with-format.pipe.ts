import { Pipe, PipeTransform } from "@angular/core";
import * as dayjs from "dayjs";
import { CONFIG } from "../constants/config.constants";

@Pipe({
  name: "dateWithFormat",
})
export class DateWithFormatPipe implements PipeTransform {
  transform(value: any, format: string): any {
    if (value) {
      return dayjs(value).format(format);
    } else {
      return "";
    }
  }
}
