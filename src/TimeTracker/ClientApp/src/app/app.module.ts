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
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [
    AppComponent,
    SideNavComponent,
    HeaderComponent,
    HomeComponent,
    TestComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MaterialModule,
    TimeTrackerModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
