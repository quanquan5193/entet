import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/share/shared.module';
import { EditKidClubComponent } from './edit/edit.component';
import { KidClubRoutingModule } from './kid-club-routing.module';
import { ListViewKidClubComponent } from './list-view/list-view.component';
import { ViewKidClubComponent } from './view/view.component';

const COMPONENTS = [
  ListViewKidClubComponent,
  ViewKidClubComponent,
  EditKidClubComponent
];

const ENTRY_COMPONENTS = [];

const MODULES = [
  KidClubRoutingModule,
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
export class KidClubModule { }
