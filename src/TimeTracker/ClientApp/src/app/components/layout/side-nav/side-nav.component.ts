import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-side-nav',
  templateUrl: './side-nav.component.html',
  styleUrls: ['./side-nav.component.css']
})
export class SideNavComponent implements OnInit, OnDestroy {
  @Output() closeSidenav = new EventEmitter<void>();
  loggedIn: boolean = false;

  private subscriptions: Subscription[] = [];
  
  constructor(
    private _authService: AuthService
  ) { }
  
  ngOnInit(): void {
    this.loggedIn = this._authService.loggedIn;

    this.subscriptions.push(this._authService.authChanged.subscribe(
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

  onLogout() {
    if(confirm('Log out?') == false)
      return;

    this._authService.logout();
    this.onClose();
  }

  onClose() {
    this.closeSidenav.emit();
  }
}
