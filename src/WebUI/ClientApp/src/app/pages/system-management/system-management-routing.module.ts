import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizeGuard } from 'src/app/authorization/authorize.guard';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { CompanyManagementComponent } from './company-management/company-management.component';
import { StoreManagementComponent } from './store-management/store-management.component';
import { SystemManagementMenuComponent } from './system-management-menu/system-management-menu.component';
import { UserManagementComponent } from './user-management/user-management.component';

const permission = PERMISSION;
const routes: Routes = [
  {
    path: '',
    component: SystemManagementMenuComponent,
    data: {
      breadcrumb: {
        label: 'systemManagement.route.list',
        info: '/assets/image/icon/setting.svg'
      },
      funcId: permission.WF9
    },
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'company-management',
    component: CompanyManagementComponent,
    data: {
      breadcrumb: 'systemManagement.route.company',
      funcId: permission.WF91
    },
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'store-management',
    component: StoreManagementComponent,
    data: {
      breadcrumb: 'systemManagement.route.store',
      funcId: permission.WF92
    },
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'device-management',
    loadChildren: () => import('../system-management/device-management/device-management.module')
      .then(m => m.DeviceManagementModule),
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'user-management',
    component: UserManagementComponent,
    data: {
      breadcrumb: 'systemManagement.route.user',
      funcId: permission.WF94
    },
    canActivate: [AuthorizeGuard]
  },];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SystemManagementRoutingModule { }

export const routedComponents = [
  CompanyManagementComponent,
  StoreManagementComponent,
  UserManagementComponent
];
