import { NgModule } from "@angular/core";
import { SharedModule } from "src/app/share/shared.module";
import { CardRoutingModule } from "./card-routing.module";
import { EditCardComponent } from "./edit/edit.component";
import { ListViewCardComponent } from "./list-view/list-view.component";
import { ViewCardComponent } from "./view/view.component";

const COMPONENTS = [
  ListViewCardComponent,
  ViewCardComponent,
  EditCardComponent,
];

const ENTRY_COMPONENTS = [];

const MODULES = [CardRoutingModule, SharedModule];

const SERVICES = [];
@NgModule({
  imports: [...MODULES],
  declarations: [...COMPONENTS],
  providers: [...SERVICES],
  entryComponents: [...ENTRY_COMPONENTS],
})
export class CardModule { }
