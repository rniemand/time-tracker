import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/views/home/home.component';
import { LoginComponent } from './components/views/login/login.component';
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
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
