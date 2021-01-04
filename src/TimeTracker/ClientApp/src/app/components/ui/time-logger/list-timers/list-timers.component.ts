import { Component, OnDestroy, OnInit } from '@angular/core';
import { UiService } from 'src/app/services/ui.service';
import { RawTrackedTimeDto, TimersClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-list-timers',
  templateUrl: './list-timers.component.html',
  styleUrls: ['./list-timers.component.css']
})
export class ListTimersComponent implements OnInit, OnDestroy {
  timers: RawTrackedTimeDto[] = [];
  flipFlop: boolean = false;

  private _interval: any = null;

  constructor(
    private timersClient: TimersClient,
    private uiService: UiService
  ) { }
  
  ngOnInit(): void {
    this._interval = setInterval(() => { this.flipFlop = !this.flipFlop; }, 1000);
    this.refreshTimers();
  }

  ngOnDestroy(): void {
    if(this._interval) {
      clearInterval(this._interval);
    }
  }

  pause = (timer: RawTrackedTimeDto) => {
    let entryId = timer?.entryId ?? 0;
    if(entryId == 0) return;

    this.uiService.showLoader(true);
    this.timersClient.pauseTimer(entryId).toPromise().then(
      (updatedTimer: RawTrackedTimeDto) => {
        this.refreshTimers();
      },
      this.uiService.handleClientError
    );
  }

  resume = (timer: RawTrackedTimeDto) => {
    let entryId = timer?.entryId ?? 0;
    if(entryId == 0) return;

    this.uiService.showLoader(true);
    this.timersClient.resumeTimer(entryId).toPromise().then(
      (success: boolean) => { this.refreshTimers(); },
      this.uiService.handleClientError
    );
  }

  refresh = () => {
    this.refreshTimers();
  }


  // Internal methods
  private refreshTimers = () => {
    this.timers = [];
    this.uiService.showLoader(true);

    this.timersClient.getRunningTimers().toPromise().then(
      (timers: RawTrackedTimeDto[]) => {
        this.timers = timers;
        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );
  }

}
