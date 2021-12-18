import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizeGuard } from 'src/app/authorization/authorize.guard';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { CreateDeviceManagementComponent } from './create/create.component';
import { EditDeviceManagementComponent } from './edit/edit.component';
import { ListViewDeviceManagementComponent } from './list-view/list-view.component';

const permission = PERMISSION;
const routes: Routes = [{
  path: '',
  component: ListViewDeviceManagementComponent,
  data: {
    breadcrumb: 'deviceManagement.route.list',
    funcId: permission.WF93
  },
  canActivate: [AuthorizeGuard]
},
{
  path: ':id/edit',
  component: EditDeviceManagementComponent,
  data: {
    breadcrumb: 'deviceManagement.route.edit',
    funcId: permission.WF935
  },
  canActivate: [AuthorizeGuard]
},
{
  path: 'create',
  component: CreateDeviceManagementComponent,
  data: {
    breadcrumb: 'deviceManagement.route.create',
    funcId: permission.WF933
  },
  canActivate: [AuthorizeGuard]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DeviceManagementRoutingModule { }

export const routedComponents = [
  ListViewDeviceManagementComponent,
  EditDeviceManagementComponent,
  CreateDeviceManagementComponent
];
