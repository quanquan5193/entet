import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizeGuard } from 'src/app/authorization/authorize.guard';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { BatchProcessMenuComponent } from './batch-process-menu/batch-process-menu.component';
import { DeleteCardsComponent } from './delete-cards/delete-cards.component';
import { DeleteKidClubComponent } from './delete-kid-club/delete-kid-club.component';
import { EditCardsComponent } from './edit-cards/edit-cards.component';
import { RegisterCardsComponent } from './register-cards/register-cards.component';

const permission = PERMISSION;
const routes: Routes = [
  {
    path: '',
    component: BatchProcessMenuComponent,
    data: {
      breadcrumb: {
        label: 'batchProcess.route.menu',
        info: '/assets/image/icon/import.svg'
      }
    },
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'add-card',
    component: RegisterCardsComponent,
    data: {
      breadcrumb: 'batchProcess.route.addCards',
      funcId: permission.WF61
    },
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'edit-card',
    component: EditCardsComponent,
    data: {
      breadcrumb: 'batchProcess.route.editCards',
      funcId: permission.WF62
    },
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'delete-card',
    component: DeleteCardsComponent,
    data: {
      breadcrumb: 'batchProcess.route.deleteCards',
      funcId: permission.WF63
    },
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'delete-kid-club',
    component: DeleteKidClubComponent,
    data: {
      breadcrumb: 'batchProcess.route.deleteKid',
      funcId: permission.WF64
    },
    canActivate: [AuthorizeGuard]
  },];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class BatchProcessRoutingModule { }

export const routedComponents = [
  DeleteCardsComponent,
  RegisterCardsComponent,
  EditCardsComponent,
  DeleteKidClubComponent
];
