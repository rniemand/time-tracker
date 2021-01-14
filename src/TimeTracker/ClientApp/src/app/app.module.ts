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
import { TimeLoggerComponent } from './components/ui/time-logger/time-logger.component';
import { StartTimerComponent } from './components/ui/time-logger/start-timer/start-timer.component';
import { ProjectSelectorComponent } from './components/ui/project-selector/project-selector.component';
import { ListTimersComponent } from './components/ui/time-logger/list-timers/list-timers.component';
import { RunningTimerPipe } from './pipes/running-timer.pipe';
import { TimerSeriesDialog } from './dialogs/timer-series/timer-series.dialog';
import { EntryStatePipe } from './pipes/entry-state.pipe';
import { EditTimerEntryDialog } from './dialogs/edit-timer-entry/edit-timer-entry.dialog';
import { EditTimerEntryComponent } from './components/ui/edit-timer-entry/edit-timer-entry.component';
import { ValidationErrorDialog } from './dialogs/validation-error/validation-error.dialog';
import { PausedTimerPipe } from './pipes/paused-timer.pipe';
import { TimersComponent } from './views/timers/timers.component';
import { ClientTimersComponent } from './views/timers/client-timers/client-timers.component';
import { ProductTimersComponent } from './views/timers/product-timers/product-timers.component';
import { ProjectTimersComponent } from './views/timers/project-timers/project-timers.component';
import { TimerStatePipe } from './pipes/timer-state.pipe';
import { DailyTasksComponent } from './views/daily-tasks/daily-tasks.component';
import { AddDailyTaskComponent } from './views/daily-tasks/add-daily-task/add-daily-task.component';
import { EditDailyTaskComponent } from './views/daily-tasks/edit-daily-task/edit-daily-task.component';

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
    TimeLoggerComponent,
    StartTimerComponent,
    ProjectSelectorComponent,
    ListTimersComponent,
    RunningTimerPipe,
    TimerSeriesDialog,
    EntryStatePipe,
    EditTimerEntryDialog,
    EditTimerEntryComponent,
    ValidationErrorDialog,
    PausedTimerPipe,
    TimersComponent,
    ClientTimersComponent,
    ProductTimersComponent,
    ProjectTimersComponent,
    TimerStatePipe,
    DailyTasksComponent,
    AddDailyTaskComponent,
    EditDailyTaskComponent
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
