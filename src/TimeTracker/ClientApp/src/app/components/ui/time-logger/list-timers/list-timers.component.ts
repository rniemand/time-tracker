import { Component, OnDestroy, OnInit } from '@angular/core';
import { UiService } from 'src/app/services/ui.service';
import { RawTrackedTimeDto, TrackedTimeClient } from 'src/app/time-tracker-api';

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
    private trackedTimeClient: TrackedTimeClient,
    private uiService: UiService
  ) { }
  
  ngOnInit(): void {
    this.timers = [];

    this._interval = setInterval(() => { this.flipFlop = !this.flipFlop; }, 1000);

    this.trackedTimeClient.getRunningTimers().toPromise().then(
      (timers: RawTrackedTimeDto[]) => {
        this.timers = timers;
      },
      this.uiService.handleClientError
    );
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
    this.trackedTimeClient.pauseTimer(entryId).toPromise().then(
      (updatedTimer: RawTrackedTimeDto) => {
        console.log(updatedTimer);
        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );
  }

  resume = (timer: RawTrackedTimeDto) => {
    console.log('resume', timer);
  }

}
