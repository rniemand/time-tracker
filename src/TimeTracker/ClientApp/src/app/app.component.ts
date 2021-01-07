import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { UiService } from './services/ui.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Time Tracker';
  showLoader: boolean = false;

  private subscriptions: Subscription[] = [];

  constructor(
    private uiService: UiService
  ) { }

  ngOnInit(): void {
    this.showLoader = this.uiService.loaderVisible;
    
    this.subscriptions.push(this.uiService.showLoaderChange.subscribe(
      (visible: boolean) => { this.showLoader = visible; }
    ));
  }
}
