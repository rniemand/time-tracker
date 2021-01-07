import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { AuthService, UserInfo } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {
  username = new FormControl('', [Validators.required, Validators.minLength(1)]);
  password = new FormControl('', [Validators.required, Validators.minLength(1)]);
  loggedIn: boolean = false;
  currentUser?: UserInfo;

  private subscriptions: Subscription[] = [];

  constructor(
    private _authService: AuthService
  ) { }
  
  ngOnInit(): void {
    this.loggedIn = this._authService.loggedIn;
    this.currentUser = this._authService.currentUser;

    this.subscriptions.push(this._authService.authChanged.subscribe(
      (loggedIn: boolean) => {
        this.loggedIn = loggedIn;
        this.currentUser = this._authService.currentUser;
      }
    ));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub: Subscription) => {
      sub?.unsubscribe();
    });
  }

  validCreds = () => {
    if(this.username.invalid || this.password.invalid)
      return true;

    return false;
  }

  login = () => {
    this._authService.login(this.username.value, this.password.value);
  }

  logout = () => {
    this._authService.logout();
  }
}
