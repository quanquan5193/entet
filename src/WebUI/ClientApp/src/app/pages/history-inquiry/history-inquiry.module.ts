import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/share/shared.module';
import { EditHistoryInquiryComponent } from './edit/edit.component';
import { HistoryInquiryRoutingModule } from './history-inquiry-routing.module';
import { ListViewHistoryInquiryComponent } from './list-view/list-view.component';
import { ViewHistoryInquiryComponent } from './view/view.component';

const COMPONENTS = [
  ListViewHistoryInquiryComponent,
  ViewHistoryInquiryComponent,
  EditHistoryInquiryComponent
];

const ENTRY_COMPONENTS = [];

const MODULES = [
  HistoryInquiryRoutingModule,
  SharedModule,
];

const SERVICES = [
];
@NgModule({
  imports: [
    ...MODULES,
  ],
  declarations: [
    ...COMPONENTS,
  ],
  providers: [
    ...SERVICES,
  ],
  entryComponents: [
    ...ENTRY_COMPONENTS,
  ],
})
export class HistoryInquiryModule { }
