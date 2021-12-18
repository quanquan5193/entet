import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/share/shared.module';
import { CloseDeviceModalComponent } from './closeDeviceModal/close-device-modal.component';
import { ConfirmCreateModalComponent } from './create/confirm-modal/confirm-create-modal.component';
import { CreateDeviceManagementComponent } from './create/create.component';
import { DeviceManagementRoutingModule } from './device-management-routing.module';
import { EditDeviceManagementComponent } from './edit/edit.component';
import { ListViewDeviceManagementComponent } from './list-view/list-view.component';

const COMPONENTS = [
  ListViewDeviceManagementComponent,
  EditDeviceManagementComponent,
  CreateDeviceManagementComponent,
  ConfirmCreateModalComponent,
  CloseDeviceModalComponent
];

const ENTRY_COMPONENTS = [];

const MODULES = [
  DeviceManagementRoutingModule,
  SharedModule
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
export class DeviceManagementModule { }
