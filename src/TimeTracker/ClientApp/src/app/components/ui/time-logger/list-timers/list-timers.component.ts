import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TimerSeriesDialog, TimerSeriesDialogData } from 'src/app/dialogs/timer-series/timer-series.dialog';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { LoggerService } from 'src/app/services/logger.service';
import { StorageService } from 'src/app/services/storage.service';
import { UiService } from 'src/app/services/ui.service';
import { RawTimerDto, TimersClient } from 'src/app/time-tracker-api';

const KEY_STATE = 'tt.list_timer.state';

interface ListTimersState {
  autoRefresh: boolean;
}

@Component({
  selector: 'app-list-timers',
  templateUrl: './list-timers.component.html',
  styleUrls: ['./list-timers.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class ListTimersComponent implements OnInit, OnDestroy {
  timers: RawTimerDto[] = [];
  flipFlop: boolean = false;
  remaining: number = 30;
  autoRefresh: boolean = true;
  runningTimers: boolean = false;

  private _interval: any = null;
  private _decrementTimer: boolean = true;

  constructor(
    private timersClient: TimersClient,
    private uiService: UiService,
    public dialog: MatDialog,
    private storage: StorageService,
    private logger: LoggerService
  ) { }
  
  ngOnInit(): void {
    this.loadSavedState();
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
      (updatedTimer: boolean) => {
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

  resumeSingleTimer = (timer: RawTimerDto) => {
    let rawTimerId = timer?.rawTimerId ?? 0;
    if(rawTimerId === 0)
      return;
    
    this.uiService.showLoader(true);
    this.timersClient.resumeSingleTimer(rawTimerId).toPromise().then(
      (success: boolean) => {
        this.uiService.notify(success ? 'Timer resumed' : 'Resume failed');
        this.refreshTimers();
      },
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

  getTooltip = (timer: RawTimerDto) => {
    return `Client: ${timer?.clientName ?? 'Unknown'}`;
  }


  // Internal methods
  private refreshTimers = () => {
    this.saveCurrentState();
    this.timers = [];
    this.runningTimers = false;
    this.uiService.showLoader(true);

    this.timersClient.getActiveTimers().toPromise().then(
      (timers: RawTimerDto[]) => {
        this.timers = timers;
        this.remaining = 30;
        this._decrementTimer = true;
        this.runningTimers = this.containsRunningTimers(timers);
        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );
  }

  private tick = () => {
    this.flipFlop = !this.flipFlop;

    if(this._decrementTimer) {
      this.remaining -= 1;
    }

    if(this.remaining % 5 === 0) {
      this.saveCurrentState();
    }

    if(this.autoRefresh && this.remaining == 0) {
      this.remaining = -1;
      this._decrementTimer = false;
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

  private containsRunningTimers = (timers: RawTimerDto[]) => {
    if(timers.length === 0)
      return false;

    for(var i = 0; i < timers.length; i++) {
      if(timers[i].entryState == 1) {
        return true;
      }
    }

    return false;
  }

  // State management
  private saveCurrentState = () => {
    let state: ListTimersState = {
      autoRefresh: this.autoRefresh
    };

    this.storage.setItem(KEY_STATE, state);
    this.logger.trace('list timers state saved');
  }

  private loadSavedState = () => {
    let state: ListTimersState = {
      autoRefresh: true
    };

    if(this.storage.hasItem(KEY_STATE)) {
      state = this.storage.getItem<ListTimersState>(KEY_STATE);
      this.logger.trace('saved list timers state loaded');
    }

    this.autoRefresh = state.autoRefresh;
  }
}
