import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AddClientComponent } from './views/clients/add-client/add-client.component';
import { ClientsComponent } from './views/clients/clients.component';
import { EditClientComponent } from './views/clients/edit-client/edit-client.component';
import { HomeComponent } from './views/home/home.component';
import { LoginComponent } from './views/login/login.component';
import { AddProductComponent } from './views/products/add-product/add-product.component';
import { EditProductComponent } from './views/products/edit-product/edit-product.component';
import { ProductsComponent } from './views/products/products.component';
import { AddProjectComponent } from './views/projects/add-project/add-project.component';
import { EditProjectComponent } from './views/projects/edit-project/edit-project.component';
import { ProjectsComponent } from './views/projects/projects.component';
import { AuthGuard } from './providers/append-token.interceptor';
import { DailyTasksComponent } from './views/daily-tasks/daily-tasks.component';
import { AddDailyTaskComponent } from './views/daily-tasks/add-daily-task/add-daily-task.component';
import { EditDailyTaskComponent } from './views/daily-tasks/edit-daily-task/edit-daily-task.component';
import { TimesheetComponent } from './views/timesheet/timesheet.component';

const routes: Routes = [
  { path: "", component: HomeComponent },
  { path: "home", component: HomeComponent },
  { path: "login", component: LoginComponent },
  {
    path: "clients",
    canActivate: [AuthGuard],
    children: [
      { path: "", component: ClientsComponent },
      { path: "add", component: AddClientComponent },
      { path: "edit/:id", component: EditClientComponent }
    ]
  },
  {
    path: "products",
    canActivate: [AuthGuard],
    children: [
      { path: "", component: ProductsComponent },
      { path: ":clientId", component: ProductsComponent },
      { path: "add/:clientId", component: AddProductComponent },
      { path: "edit/:productId", component: EditProductComponent }
    ]
  },
  {
    path: "projects",
    canActivate: [AuthGuard],
    children: [
      { path: '', component: ProjectsComponent },
      { path: 'edit/:projectId', component: EditProjectComponent },
      { path: ':clientId/:productId', component: ProjectsComponent },
      { path: 'add/:clientId/:productId', component: AddProjectComponent }
    ]
  },
  {
    path: 'daily-tasks',
    canActivate: [AuthGuard],
    children: [
      { path: '', component: DailyTasksComponent },
      { path: ':clientId', component: DailyTasksComponent },
      { path: 'add/:clientId', component: AddDailyTaskComponent },
      { path: 'edit/:taskId', component: EditDailyTaskComponent }
    ]
  },
  {
    path: 'timesheet',
    canActivate: [AuthGuard],
    children: [
      { path: '', component: TimesheetComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
