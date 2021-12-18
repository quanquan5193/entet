import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CommonMethodsService {

  constructor() {
  }

  getMimeType(fileExtension: string) {
    switch (fileExtension.toLocaleLowerCase()) {
      case '.doc':
        return 'application/msword';
      case '.dot':
        return 'application/msword';
      case '.docx':
        return 'application/vnd.openxmlformats-officedocument.wordprocessingml.document';
      case '.dotx':
        return 'application/vnd.openxmlformats-officedocument.wordprocessingml.template';
      case '.docm':
        return 'application/vnd.ms-word.document.macroEnabled.12';
      case '.dotm':
        return 'application/vnd.ms-word.template.macroEnabled.12';
      case '.xls':
        return 'application/vnd.ms-excel';
      case '.xlt':
        return 'application/vnd.ms-excel';
      case '.xla':
        return 'application/vnd.ms-excel';
      case '.xlsx':
        return 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
      case '.xltx':
        return 'application/vnd.openxmlformats-officedocument.spreadsheetml.template';
      case '.xlsm':
        return 'application/vnd.ms-excel.sheet.macroEnabled.12';
      case '.xltm':
        return 'application/vnd.ms-excel.template.macroEnabled.12';
      case '.xlam':
        return 'application/vnd.ms-excel.addin.macroEnabled.12';
      case '.xlsb':
        return 'application/vnd.ms-excel.sheet.binary.macroEnabled.12';
      case '.ppt':
        return 'application/vnd.ms-powerpoint';
      case '.pot':
        return 'application/vnd.ms-powerpoint';
      case '.pps':
        return 'application/vnd.ms-powerpoint';
      case '.ppa':
        return 'application/vnd.ms-powerpoint';
      case '.pptx':
        return 'application/vnd.openxmlformats-officedocument.presentationml.presentation';
      case '.potx':
        return 'application/vnd.openxmlformats-officedocument.presentationml.template';
      case '.ppsx':
        return 'application/vnd.openxmlformats-officedocument.presentationml.slideshow';
      case '.ppam':
        return 'application/vnd.ms-powerpoint.addin.macroEnabled.12';
      case '.pptm':
        return 'application/vnd.ms-powerpoint.presentation.macroEnabled.12';
      case '.potm':
        return 'application/vnd.ms-powerpoint.template.macroEnabled.12';
      case '.ppsm':
        return 'application/vnd.ms-powerpoint.slideshow.macroEnabled.12';
      case '.zip':
        return 'application/zip';
      case '.rar':
        return 'application/x-rar-compressed';
      default:
        return '';
    }
  }

  getFileTypeFromName(fileName: string) {
    const temp = fileName.slice(0, fileName.lastIndexOf('.') + 1);
    const extension = fileName.replace(temp, '');
    return extension.toLowerCase();
  }

  checkFileTypeAndSize(file: any) {
    const ext = this.getFileTypeFromName(file.name);
    const types = ['csv', 'doc', 'docm', 'docx', 'dot', 'dotm', 'dotx', 'pdf', 'pot', 'potm',
      'potx', 'ppa', 'ppam', 'pps', 'ppsm', 'ppsx', 'ppt', 'pptm', 'pptx', 'rtf',
      'txt', 'xla', 'xlam', 'xls', 'xlsb', 'xlsm', 'xlsx', 'xlt', 'xltm', 'xltx',
      'xlw', 'zip', 'rar'];
    if (types.indexOf(ext) === -1) {
      return false;
    } else if (file.size > 10485760) {
      return false;
    }
    return true;
  }

  camelCaseToPascalCaseTo(propname) {
    return propname.charAt(0).toUpperCase() + propname.slice(1);
  }

  convertPropertyNames(obj, converterFn) {
    var r, value, t = Object.prototype.toString.apply(obj);
    if (t == "[object Object]") {
      r = {};
      for (var propname in obj) {
        value = obj[propname];
        r[converterFn(propname)] = this.convertPropertyNames(value, converterFn);
      }
      return r;
    }
    else if (t == "[object Array]") {
      r = [];
      for (var i = 0, L = obj.length; i < L; ++i) {
        value = obj[i];
        r[i] = this.convertPropertyNames(value, converterFn);
      }
      return r;
    }
    return obj;
  }
}
