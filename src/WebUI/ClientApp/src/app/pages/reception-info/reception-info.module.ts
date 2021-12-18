import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/share/shared.module';
import { ListViewReceptionInfoComponent } from './list-view/list-view-reception-info.component';
import { ReceptionInfoRoutingModule } from './reception-info-routing.module';


@NgModule({
  imports: [
    ReceptionInfoRoutingModule,
    SharedModule
  ],
  declarations: [ListViewReceptionInfoComponent],
})
export class ReceptionInfoModule { }
