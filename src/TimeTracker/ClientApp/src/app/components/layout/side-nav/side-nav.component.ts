import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-side-nav',
  templateUrl: './side-nav.component.html',
  styleUrls: ['./side-nav.component.css']
})
export class SideNavComponent implements OnInit {
  @Output() closeSidenav = new EventEmitter<void>();
  loggedIn: boolean = false;
  
  constructor() { }

  ngOnInit(): void {
  }

  onLogout() {
    // TODO: [COMPLETE] Complete me
  }

  onClose() {
    this.closeSidenav.emit();
  }

}
