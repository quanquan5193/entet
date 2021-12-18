import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { MainLayoutComponent } from './main-layout/main-layout';
import { NotFoundComponent } from '../share/component/not-found/not-found.component';
import { AuthorizeGuard } from '../authorization/authorize.guard';
import { HomeComponent } from '../pages/home/home.component';
import { TimeoutComponent } from '../share/component/timeout/timeout.component';
import { AccessDeniedComponent } from '../share/component/access-denied/access-denied.component';
import { PERMISSION } from '../share/constants/permission.constants';

const permission = PERMISSION;
const routes: Routes = [{
  path: '',
  component: MainLayoutComponent,
  children: [
    {
      path: 'home',
      component: HomeComponent,
      data: {
        breadcrumb: {
          skip: true
        }
      },
      canActivate: [AuthorizeGuard]
    },
    {
      path: 'timeout',
      component: TimeoutComponent,
      data: {
        breadcrumb: {
          skip: true
        }
      },
      canActivate: [AuthorizeGuard]
    },
    {
      path: 'access-denied',
      component: AccessDeniedComponent,
      data: {
        breadcrumb: {
          skip: true
        }
      },
      canActivate: [AuthorizeGuard]
    },
    {
      path: 'card',
      loadChildren: () => import('../pages/card/card.module')
        .then(m => m.CardModule),
      canActivate: [AuthorizeGuard],
      data: {
        funcId: permission.WF4
      }
    },
    {
      path: 'kid-club',
      loadChildren: () => import('../pages/kid-club/kid-club.module')
        .then(m => m.KidClubModule),
      canActivate: [AuthorizeGuard],
      data: {
        funcId: permission.WF5
      }
    },
    {
      path: 'batch-process',
      loadChildren: () => import('../pages/batch-process/batch-process.module')
        .then(m => m.BatchProcessModule),
      canActivate: [AuthorizeGuard],
      data: {
        funcId: permission.WF6
      }
    },
    {
      path: 'history-inquiry',
      loadChildren: () => import('../pages/history-inquiry/history-inquiry.module')
        .then(m => m.HistoryInquiryModule),
      canActivate: [AuthorizeGuard],
      data: {
        funcId: permission.WF7
      }
    },
    {
      path: 'reception-info',
      loadChildren: () => import('../pages/reception-info/reception-info.module')
        .then(m => m.ReceptionInfoModule),
      canActivate: [AuthorizeGuard],
      data: {
        funcId: permission.WF8
      }
    },
    {
      path: 'system-management',
      loadChildren: () => import('../pages/system-management/system-management.module')
        .then(m => m.SystemManagementModule),
      canActivate: [AuthorizeGuard],
      data: {
        funcId: permission.WF9
      }
    },
    {
      path: '',
      redirectTo: 'home',
      pathMatch: 'full'
    },
    {
      path: '**',
      component: NotFoundComponent,
      data: {
        breadcrumb: {
          skip: true
        }
      },
    },
  ],
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LayoutRoutingModule {
}
