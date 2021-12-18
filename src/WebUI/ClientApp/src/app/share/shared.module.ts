import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { NgbDateAdapter, NgbDateNativeAdapter, NgbDateParserFormatter, NgbDatepickerI18n, NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { TranslateModule } from "@ngx-translate/core";
import { AgGridModule } from "ag-grid-angular";
import { CommonGridComponent } from "./component/common-grid/common-grid.component";
import { CloseModalComponent } from "./modal/close-modal/close-modal.component";
import { ConfirmModalComponent } from "./modal/confirm-modal/confirm-modal.component";
import { NotFoundComponent } from "./component/not-found/not-found.component";
import { DateWithFormatPipe } from "./pipes/date-with-format.pipe";
import { DateFormatPipe } from "./pipes/date-format.pipe";
import { DateTimeFormatPipe } from "./pipes/datetime-format.pipe";
import { DateTimeFullFormatPipe } from "./pipes/datetime-full-format.pipe";
import { JapaneseLayoutDatepickerDirective } from "./directives/japanese-layout-datepicker.directive";
import { CustomDatepickerI18n, I18n, NgBootstrapDateFormatterFactory } from "./factories/ng-bootstrap-date-formatter.factory";
import { DateTypeValidatorDirective } from "./directives/date-type.validation.directive";
import { NoticeModalComponent } from "./modal/notice-modal/notice-modal.component";
import { NgSelectModule } from "@ng-select/ng-select";
import { UiSwitchModule } from "ngx-toggle-switch";
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { PermissionCheckDirective } from "./directives/permission-check.directive";
import { TimeoutComponent } from "./component/timeout/timeout.component";
import { AccessDeniedComponent } from "./component/access-denied/access-denied.component";
import { ToLocalDateFormatPipe } from "./pipes/to-local-time";
import { OnlyHalfWidthDirective } from "./directives/only-half-with.directive";

const COMPONENTS = [
  NotFoundComponent,
  CloseModalComponent,
  ConfirmModalComponent,
  NoticeModalComponent,
  DateFormatPipe,
  DateTimeFormatPipe,
  DateTimeFullFormatPipe,
  ToLocalDateFormatPipe,
  CommonGridComponent,
  JapaneseLayoutDatepickerDirective,
  DateTypeValidatorDirective,
  DateWithFormatPipe,
  PermissionCheckDirective,
  OnlyHalfWidthDirective,
  TimeoutComponent,
  AccessDeniedComponent
];

const ENTRY_COMPONENTS = [
  CloseModalComponent,
  ConfirmModalComponent,
  NoticeModalComponent,
];

const EXPORT = [
  TranslateModule,
  CommonModule,
  FormsModule,
  NgbModule,
  CloseModalComponent,
  ConfirmModalComponent,
  NoticeModalComponent,
  DateFormatPipe,
  DateTimeFormatPipe,
  DateTimeFullFormatPipe,
  ToLocalDateFormatPipe,
  CommonGridComponent,
  JapaneseLayoutDatepickerDirective,
  DateTypeValidatorDirective,
  DateWithFormatPipe,
  NgSelectModule,
  UiSwitchModule,
  NgxChartsModule,
  OnlyHalfWidthDirective,
  PermissionCheckDirective
];

const MODULES = [
  TranslateModule,
  CommonModule,
  FormsModule,
  NgbModule,
  FontAwesomeModule,
  AgGridModule.withComponents([]),
  NgSelectModule,
  UiSwitchModule,
  NgxChartsModule,
];

const SERVICES = [
  I18n,
  { provide: NgbDateParserFormatter, useClass: NgBootstrapDateFormatterFactory },
  { provide: NgbDatepickerI18n, useClass: CustomDatepickerI18n },
  { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter },
];

@NgModule({
  imports: [...MODULES],
  declarations: [...COMPONENTS],
  providers: [...SERVICES],
  entryComponents: [...ENTRY_COMPONENTS],
  exports: [...EXPORT],
})
export class SharedModule { }
