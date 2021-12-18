import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ModalModule } from 'ngx-bootstrap/modal';
import { LoaderInterceptor } from './share/interceptors/loader.interceptor';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { AppRoutingModule } from './app-routing.module';
import { SpinnerComponent } from './share/component/spinner/spinner.component';
import { LayoutModule } from './layout/layout-component.module';
import { ApiAuthorizationModule } from './authorization/api-authorization.module';
import { AuthorizeInterceptor } from './authorization/authorize.interceptor';
// import { environment } from 'src/environments/environment';
// import { API_BASE_URL } from 'src/app/web-api-client'

export function HttpLoaderFactory(httpClient: HttpClient) {
  return new TranslateHttpLoader(httpClient);
}

const COMPONENTS = [
  AppComponent,
  SpinnerComponent
];

const MODULES = [
  BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
  FontAwesomeModule,
  HttpClientModule,
  ApiAuthorizationModule,
  FormsModule,
  BrowserAnimationsModule,
  TranslateModule.forRoot({
    loader: {
      provide: TranslateLoader,
      useFactory: HttpLoaderFactory,
      deps: [HttpClient]
    }
  }),
  ModalModule.forRoot(),
  LayoutModule,
  NgbModule,
  AppRoutingModule
];

const SERVICES = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true, },
];

const BOOTSTRAP = [
  AppComponent
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
    // {
    //   provide: API_BASE_URL,
    //   useValue: environment.API_BASE_URL
    // },
  ],
  bootstrap: [
    ...BOOTSTRAP,
  ]
})

export class AppModule {
  constructor (translate: TranslateService) {
    // this language will be used as a fallback when a translation isn't found in the current language
    translate.setDefaultLang('ja');

    // the lang to use, if the lang isn't available, it will use the current loader to get them
    translate.use('ja');
  }
}
