import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SideNavComponent } from './components/layout/side-nav/side-nav.component';
import { HeaderComponent } from './components/layout/header/header.component';
import { MaterialModule } from './modules/material.module';
import { HomeComponent } from './components/views/home/home.component';
import { TestComponent } from './components/views/test/test.component';
import { TimeTrackerModule } from './modules/time-tracker.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppendTokenInterceptor, ErrorInterceptor } from './providers/append-token.interceptor';
import { LoginComponent } from './components/views/login/login.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ClientsComponent } from './components/views/clients/clients.component';
import { AddClientComponent } from './components/views/clients/add-client/add-client.component';
import { EditClientComponent } from './components/views/clients/edit-client/edit-client.component';

@NgModule({
  declarations: [
    AppComponent,
    SideNavComponent,
    HeaderComponent,
    HomeComponent,
    TestComponent,
    LoginComponent,
    ClientsComponent,
    AddClientComponent,
    EditClientComponent
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
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
