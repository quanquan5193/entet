import { NgModule } from "@angular/core";
import { ApiAuthorizationModule } from "../authorization/api-authorization.module";
import { SharedModule } from "../share/shared.module";
import { HeaderComponent } from "./header/header";
import { LayoutRoutingModule } from "./layout-routing.module";
import { MainLayoutComponent } from "./main-layout/main-layout";
import { BreadcrumbModule } from "xng-breadcrumb";
import { PagesModule } from "../pages/pages.module";

const COMPONENTS = [MainLayoutComponent, HeaderComponent];

const ENTRY_COMPONENTS = [];

const MODULES = [
  LayoutRoutingModule,
  SharedModule,
  PagesModule,
  ApiAuthorizationModule,
  BreadcrumbModule,
];

const SERVICES = [];
@NgModule({
  imports: [...MODULES],
  declarations: [...COMPONENTS],
  providers: [...SERVICES],
  entryComponents: [...ENTRY_COMPONENTS],
})
export class LayoutModule {}
