import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginMenuComponent } from './login-menu/login-menu.component';
import { LoginComponent } from './login/login.component';
import { RouterModule } from '@angular/router';
import { ApplicationPaths } from './api-authorization.constants';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

const COMPONENTS = [
  LoginMenuComponent,
  LoginComponent
];

const ENTRY_COMPONENTS = [];

const EXPORT = [
  LoginMenuComponent,
  LoginComponent,
];

const MODULES = [
  CommonModule,
  NgbModule,
  ReactiveFormsModule,
  HttpClientModule,
  TranslateModule,
  RouterModule.forChild(
    [
      { path: ApplicationPaths.Register, component: LoginComponent },
      { path: ApplicationPaths.Profile, component: LoginComponent },
      { path: ApplicationPaths.Login, component: LoginComponent },
      { path: ApplicationPaths.LoginFailed, component: LoginComponent },
      { path: ApplicationPaths.LoginCallback, component: LoginComponent },
    ]
  )
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
  exports: [
    ...EXPORT
  ],
})
export class ApiAuthorizationModule { }
