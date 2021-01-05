import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TimerSeriesDialog, TimerSeriesDialogData } from 'src/app/components/dialogs/timer-series/timer-series.dialog';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { UiService } from 'src/app/services/ui.service';
import { RawTimerDto, TimersClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-list-timers',
  templateUrl: './list-timers.component.html',
  styleUrls: ['./list-timers.component.css']
})
export class ListTimersComponent implements OnInit, OnDestroy {
  timers: RawTimerDto[] = [];
  flipFlop: boolean = false;
  remaining: number = 30;
  autoRefresh: boolean = true;

  private _interval: any = null;

  constructor(
    private timersClient: TimersClient,
    private uiService: UiService,
    public dialog: MatDialog
  ) { }
  
  ngOnInit(): void {
    this.startTicker();
    this.refreshTimers();
  }

  ngOnDestroy(): void {
    this.stopTicker();
  }


  // Template methods
  pause = (timer: RawTimerDto) => {
    let entryId = timer?.rawTimerId ?? 0;
    if(entryId == 0) return;

    this.uiService.showLoader(true);
    this.timersClient.pauseTimer(entryId).toPromise().then(
      (updatedTimer: RawTimerDto) => {
        this.refreshTimers();
      },
      this.uiService.handleClientError
    );
  }

  resume = (timer: RawTimerDto) => {
    let entryId = timer?.rawTimerId ?? 0;
    if(entryId == 0) return;

    this.uiService.showLoader(true);
    this.timersClient.resumeTimer(entryId).toPromise().then(
      (success: boolean) => { this.refreshTimers(); },
      this.uiService.handleClientError
    );
  }

  stop = (timer: RawTimerDto) => {
    let rawTimerId = timer?.rawTimerId ?? 0;
    if(rawTimerId == 0)
      return;

    if(!confirm(`Stop timer: ${timer.projectName} (${timer.productName})?`)) {
      return;
    }

    this.uiService.showLoader(true);
    this.timersClient.stopTimer(rawTimerId).toPromise().then(
      (success: boolean) => {
        this.refreshTimers();
        if(success) { this.uiService.notify('Timer stopped'); }
      },
      this.uiService.handleClientError
    );
  }

  refresh = () => {
    this.refreshTimers();
  }

  getClass = (timer: RawTimerDto) => {
    if(timer.entryState === 1) {
      return ['timer-entry', 'running'];
    }

    return ['timer-entry'];
  }

  showSeries = (timer: RawTimerDto) => {
    let dialogData: TimerSeriesDialogData = {
      rootTimerId: timer?.rootTimerId ?? 0
    };

    this.stopTicker();
    let dialogRef = this.dialog.open(TimerSeriesDialog, {
      ...DIALOG_DEFAULTS,
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      this.startTicker();
    });
  }


  // Internal methods
  private refreshTimers = () => {
    this.timers = [];
    this.uiService.showLoader(true);

    this.timersClient.getRunningTimers().toPromise().then(
      (timers: RawTimerDto[]) => {
        this.timers = timers;
        this.remaining = 30;
        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );
  }

  private tick = () => {
    this.flipFlop = !this.flipFlop;
    this.remaining -= 1;

    if(this.remaining == 0) {
      this.refreshTimers();
    }
  }

  private startTicker = () => {
    this._interval = setInterval(this.tick, 1000);
  }

  private stopTicker = () => {
    if(this._interval) {
      clearInterval(this._interval);
      this._interval = null;
    }
  }

}
