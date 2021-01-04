import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { UiService } from 'src/app/services/ui.service';
import { RawTrackedTimeDto, TimersClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-start-timer',
  templateUrl: './start-timer.component.html',
  styleUrls: ['./start-timer.component.css']
})
export class StartTimerComponent implements OnInit {
  @Output() timerCreated = new EventEmitter<void>();
  clientId: number = 0;
  productId: number = 0;
  projectId: number = 0;

  constructor(
    private timersClient: TimersClient,
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
    this.timersClient.startNewTimer(newEntry).toPromise().then(
      (entry: RawTrackedTimeDto) => {
        this.timerCreated.emit();
      },
      this.uiService.handleClientError
    );
  }

}
