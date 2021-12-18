import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizeGuard } from 'src/app/authorization/authorize.guard';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { EditKidClubComponent } from './edit/edit.component';
import { ListViewKidClubComponent } from './list-view/list-view.component';
import { ViewKidClubComponent } from './view/view.component';

const permission = PERMISSION;
const routes: Routes = [{
  path: '',
  component: ListViewKidClubComponent,
  data: {
    breadcrumb: {
      label: 'kidClub.route.list',
      info: '/assets/image/icon/kid-club.svg'
    },
    funcId: permission.WF511
  },
  canActivate: [AuthorizeGuard]
},
{
  path: ':id/edit',
  component: EditKidClubComponent,
  data: {
    breadcrumb: {
      skip: true
    },
    funcId: permission.WF53
  },
  canActivate: [AuthorizeGuard]
},
{
  path: ':id',
  component: ViewKidClubComponent,
  data: {
    breadcrumb: {
      skip: true
    },
    funcId: permission.WF52
  },
  canActivate: [AuthorizeGuard]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class KidClubRoutingModule { }

export const routedComponents = [
  ListViewKidClubComponent,
  ViewKidClubComponent,
  EditKidClubComponent
];
