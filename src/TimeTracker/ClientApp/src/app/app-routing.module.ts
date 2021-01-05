import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AddClientComponent } from './components/views/clients/add-client/add-client.component';
import { ClientsComponent } from './components/views/clients/clients.component';
import { EditClientComponent } from './components/views/clients/edit-client/edit-client.component';
import { HomeComponent } from './components/views/home/home.component';
import { LoginComponent } from './components/views/login/login.component';
import { AddProductComponent } from './components/views/products/add-product/add-product.component';
import { EditProductComponent } from './components/views/products/edit-product/edit-product.component';
import { ProductsComponent } from './components/views/products/products.component';
import { AddProjectComponent } from './components/views/projects/add-project/add-project.component';
import { EditProjectComponent } from './components/views/projects/edit-project/edit-project.component';
import { ProjectsComponent } from './components/views/projects/projects.component';
import { AuthGuard } from './providers/append-token.interceptor';

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
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
