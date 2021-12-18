import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizeGuard } from 'src/app/authorization/authorize.guard';
import { PERMISSION } from 'src/app/share/constants/permission.constants';
import { EditCardComponent } from './edit/edit.component';
import { ListViewCardComponent } from './list-view/list-view.component';
import { ViewCardComponent } from './view/view.component';

const permission = PERMISSION;
const routes: Routes = [{
  path: '',
  component: ListViewCardComponent,
  data: {
    breadcrumb: {
      label: 'card.route.list',
      info: '/assets/image/icon/card.svg'
    },
    funcId: permission.WF41
  },
  canActivate: [AuthorizeGuard]
},
{
  path: ':id/edit',
  component: EditCardComponent,
  data: {
    breadcrumb: {
      skip: true
    },
    funcId: permission.WF43
  },
  canActivate: [AuthorizeGuard]
},
{
  path: ':id',
  component: ViewCardComponent,
  data: {
    breadcrumb: {
      skip: true
    },
    funcId: permission.WF42
  },
  canActivate: [AuthorizeGuard]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CardRoutingModule { }

export const routedComponents = [
  ListViewCardComponent,
  ViewCardComponent,
  EditCardComponent
];
