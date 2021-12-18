import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizeGuard } from 'src/app/authorization/authorize.guard';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { EditHistoryInquiryComponent } from './edit/edit.component';
import { ListViewHistoryInquiryComponent } from './list-view/list-view.component';
import { ViewHistoryInquiryComponent } from './view/view.component';

const permission = PERMISSION;
const routes: Routes = [{
  path: '',
  component: ListViewHistoryInquiryComponent,
  data: {
    breadcrumb: {
      label: 'historyInquiry.route.list',
      info: '/assets/image/icon/log.svg'
    },
    funcId: permission.WF71
  },
  canActivate: [AuthorizeGuard]
},
{
  path: ':id/edit',
  component: EditHistoryInquiryComponent,
  data: {
    breadcrumb: 'historyInquiry.route.edit',
    funcId: permission.WF73
  },
  canActivate: [AuthorizeGuard]
},
{
  path: ':id',
  component: ViewHistoryInquiryComponent,
  data: {
    breadcrumb: 'historyInquiry.route.view',
    funcId: permission.WF72
  },
  canActivate: [AuthorizeGuard]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HistoryInquiryRoutingModule { }

export const routedComponents = [
  ListViewHistoryInquiryComponent,
  ViewHistoryInquiryComponent,
  EditHistoryInquiryComponent
];
