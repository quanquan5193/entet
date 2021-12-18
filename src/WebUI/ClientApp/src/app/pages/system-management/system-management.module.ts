import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/share/shared.module';
import { CompanyManagementComponent } from './company-management/company-management.component';
import { StoreManagementComponent } from './store-management/store-management.component';
import { SystemManagementMenuComponent } from './system-management-menu/system-management-menu.component';
import { SystemManagementRoutingModule } from './system-management-routing.module';
import { DeviceManagementModule } from './device-management/device-management.module';
import { UserManagementComponent } from './user-management/user-management.component';
import { ConfirmCreateCompanyModalComponent } from './company-management/comfirm-create-company/confirm-create-company.component';
import { ConfirmCreateStoreModalComponent } from './store-management/confirm-create-store/confirm-create-store.component';
import { ConfirmCreateUserModalComponent } from './user-management/comfirm-create-user/confirm-create-user.component';
import { ReactiveFormsModule } from '@angular/forms';
const COMPONENTS = [
  UserManagementComponent,
  StoreManagementComponent,
  CompanyManagementComponent,
  SystemManagementMenuComponent,
  ConfirmCreateCompanyModalComponent,
  ConfirmCreateStoreModalComponent,
  ConfirmCreateUserModalComponent
];

const ENTRY_COMPONENTS = [];

const MODULES = [
  SystemManagementRoutingModule,
  DeviceManagementModule,
  SharedModule
];

const SERVICES = [];

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
export class SystemManagementModule { }
