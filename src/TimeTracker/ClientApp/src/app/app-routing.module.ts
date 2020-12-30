import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AddClientComponent } from './components/views/clients/add-client/add-client.component';
import { ClientsComponent } from './components/views/clients/clients.component';
import { EditClientComponent } from './components/views/clients/edit-client/edit-client.component';
import { HomeComponent } from './components/views/home/home.component';
import { LoginComponent } from './components/views/login/login.component';
import { ProductsComponent } from './components/views/products/products.component';
import { TestComponent } from './components/views/test/test.component';
import { AuthGuard } from './providers/append-token.interceptor';

const routes: Routes = [
  { 
    path: "", 
    component: HomeComponent
  },
  { 
    path: "home", 
    component: HomeComponent
  },
  { 
    path: "test", 
    component: TestComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: "login", 
    component: LoginComponent
  },
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
      { path: "", component: ProductsComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
