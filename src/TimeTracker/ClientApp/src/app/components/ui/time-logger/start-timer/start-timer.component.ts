import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { UiService } from 'src/app/services/ui.service';
import { RawTrackedTimeDto, TrackedTimeClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-start-timer',
  templateUrl: './start-timer.component.html',
  styleUrls: ['./start-timer.component.css']
})
export class StartTimerComponent implements OnInit {
  clientId: number = 0;
  productId: number = 0;
  projectId: number = 0;

  constructor(
    private trackedTimeClient: TrackedTimeClient,
    private authService: AuthService,
    private uiService: UiService
  ) { }

  ngOnInit(): void {
  }

  onProjectChange = () => {
  }

  startTimer = () => {
    let newEntry = new RawTrackedTimeDto({
      'parentEntryId': 0,
      'rootParentEntryId': 0,
      'clientId': this.clientId,
      'productId': this.productId,
      'projectId': this.projectId,
      'userId': this.authService.currentUser?.id ?? 0
    });
    
    this.uiService.showLoader(true);
    this.trackedTimeClient.startNewTimer(newEntry).toPromise().then(
      (entry: RawTrackedTimeDto) => {
        console.log(entry);
        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );
  }

}
