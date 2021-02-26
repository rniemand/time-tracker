import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SideNavComponent } from './components/layout/side-nav/side-nav.component';
import { HeaderComponent } from './components/layout/header/header.component';
import { MaterialModule } from './modules/material.module';
import { HomeComponent } from './views/home/home.component';
import { TimeTrackerModule } from './modules/time-tracker.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppendTokenInterceptor, ErrorInterceptor, SessionTokenInterceptor } from './providers/append-token.interceptor';
import { LoginComponent } from './views/login/login.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ClientsComponent } from './views/clients/clients.component';
import { AddClientComponent } from './views/clients/add-client/add-client.component';
import { EditClientComponent } from './views/clients/edit-client/edit-client.component';
import { ProductsComponent } from './views/products/products.component';
import { ClientSelectorComponent } from './components/ui/client-selector/client-selector.component';
import { AddProductComponent } from './views/products/add-product/add-product.component';
import { EditProductComponent } from './views/products/edit-product/edit-product.component';
import { HomeCardComponent } from './components/ui/home-card/home-card.component';
import { ProjectsComponent } from './views/projects/projects.component';
import { ProductSelectorComponent } from './components/ui/product-selector/product-selector.component';
import { AddProjectComponent } from './views/projects/add-project/add-project.component';
import { EditProjectComponent } from './views/projects/edit-project/edit-project.component';
import { ProjectSelectorComponent } from './components/ui/project-selector/project-selector.component';
import { ValidationErrorDialog } from './dialogs/validation-error/validation-error.dialog';
import { TimesheetComponent } from './views/timesheet/timesheet.component';
import { AddTimesheetRowDialog } from './dialogs/add-timesheet-row/add-timesheet-row.dialog';
import { TimeEntryEditorComponent } from './components/ui/time-entry-editor/time-entry-editor.component';

@NgModule({
  declarations: [
    AppComponent,
    SideNavComponent,
    HeaderComponent,
    HomeComponent,
    LoginComponent,
    ClientsComponent,
    AddClientComponent,
    EditClientComponent,
    ProductsComponent,
    ClientSelectorComponent,
    AddProductComponent,
    EditProductComponent,
    HomeCardComponent,
    ProjectsComponent,
    ProductSelectorComponent,
    AddProjectComponent,
    EditProjectComponent,
    ProjectSelectorComponent,
    ValidationErrorDialog,
    TimesheetComponent,
    AddTimesheetRowDialog,
    TimeEntryEditorComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MaterialModule,
    TimeTrackerModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AppendTokenInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: SessionTokenInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
