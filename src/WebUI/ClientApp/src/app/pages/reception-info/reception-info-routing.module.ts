import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizeGuard } from 'src/app/authorization/authorize.guard';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { ListViewReceptionInfoComponent } from './list-view/list-view-reception-info.component';

const permission = PERMISSION;
const routes: Routes = [{
  path: '',
  component: ListViewReceptionInfoComponent,
  data: {
    breadcrumb: {
      label: 'receptionInfo.route.list',
      info: '/assets/image/icon/chart.svg'
    },
    funcId: permission.WF8
  },
  canActivate: [AuthorizeGuard]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReceptionInfoRoutingModule { }

export const routedComponents = [
  ListViewReceptionInfoComponent
];
