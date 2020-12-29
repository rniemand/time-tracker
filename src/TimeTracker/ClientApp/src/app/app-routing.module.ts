import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/views/home/home.component';
import { TestComponent } from './components/views/test/test.component';

const routes: Routes = [
  { 
    path: "", 
    component: HomeComponent
  },
  { 
    path: "test", 
    component: TestComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
