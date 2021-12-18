import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/share/shared.module';
import { BatchProcessMenuComponent } from './batch-process-menu/batch-process-menu.component';
import { BatchProcessRoutingModule } from './batch-process-routing.module';
import { DeleteCardsComponent } from './delete-cards/delete-cards.component';
import { DeleteKidClubComponent } from './delete-kid-club/delete-kid-club.component';
import { EditCardsComponent } from './edit-cards/edit-cards.component';
import { RegisterCardsComponent } from './register-cards/register-cards.component';

const COMPONENTS = [
  DeleteCardsComponent,
  RegisterCardsComponent,
  EditCardsComponent,
  DeleteKidClubComponent,
  BatchProcessMenuComponent
];

const ENTRY_COMPONENTS = [];

const MODULES = [
  BatchProcessRoutingModule,
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
export class BatchProcessModule { }
