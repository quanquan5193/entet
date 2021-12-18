import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { SharedModule } from "../share/shared.module";
import { BatchProcessModule } from "./batch-process/batch-process.module";
import { CardModule } from "./card/card.module";
import { HomeComponent } from "./home/home.component";
import { KidClubModule } from "./kid-club/kid-club.module";
import { SystemManagementModule } from "./system-management/system-management.module";

const COMPONENTS = [
  HomeComponent
];

const ENTRY_COMPONENTS = [];

const MODULES = [
  CardModule,
  KidClubModule,
  BatchProcessModule,
  SystemManagementModule,
  SharedModule,
  RouterModule,
];

const SERVICES = [];
@NgModule({
  imports: [...MODULES],
  declarations: [...COMPONENTS],
  providers: [...SERVICES],
  entryComponents: [...ENTRY_COMPONENTS],
})
export class PagesModule { }
