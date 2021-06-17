import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  loggedIn: boolean = false;
  clientId: number = 0;
  productId: number = 0;
  projectId: number = 0;

  private subscriptions: Subscription[] = [];

  constructor(
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loggedIn = this.authService.loggedIn;

    this.subscriptions.push(this.authService.authChanged.subscribe(
      (loggedIn: boolean) => {
        this.loggedIn = loggedIn;
      }
    ));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub: Subscription) => {
      sub?.unsubscribe();
    });
  }
}
